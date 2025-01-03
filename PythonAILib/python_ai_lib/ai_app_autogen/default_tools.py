import json

from typing import Annotated

import wikipedia
from selenium import webdriver
from selenium.webdriver.edge.service import Service
from selenium.webdriver.edge.options import Options
from webdriver_manager.microsoft import EdgeChromiumDriverManager

from ai_app_file.ai_app_file_util import FileUtil
from ai_app_autogen.ai_app_autogen_props import AutoGenProps
from ai_app_vector_db.ai_app_vector_db_props import VectorDBProps, VectorSearchParameter
from ai_app_langchain.langchain_vector_db import LangChainVectorDB

def create_tools(autogen_props: AutoGenProps, vector_db_props_list: list[VectorDBProps]) -> list[callable]:

    def search_wikipedia_ja(query: Annotated[str, "String to search for"], lang: Annotated[str, "Language of Wikipedia"], num_results: Annotated[int, "Maximum number of results to display"]) -> list[str]:
        """
        This function searches Wikipedia with the specified keywords and returns related articles.
        """
        # Use the Japanese version of Wikipedia
        wikipedia.set_lang(lang)
        
        # Retrieve search results
        search_results = wikipedia.search(query, results=num_results)
        
        result_texts = []
        # Display the top results
        for i, title in enumerate(search_results):
        
            print(f"Result {i + 1}: {title}")
            try:
                # Retrieve the content of the page
                page = wikipedia.page(title)
                print(page.content[:500])  # Display the first 500 characters
                text = f"Title:\n{title}\n\nContent:\n{page.content}\n"
                result_texts.append(text)
            except wikipedia.exceptions.DisambiguationError as e:
                print(f"Disambiguation: {e.options}")
            except wikipedia.exceptions.PageError:
                print("Page not found.")
            print("\n" + "-"*50 + "\n")
        return result_texts

    def list_files_in_directory(directory_path: Annotated[str, "Directory path"]) -> list[str]:
        """
        This function returns a list of files in the specified directory.
        """
        import os
        files = os.listdir(directory_path)
        return files

    def extract_file(file_path: Annotated[str, "File path"]) -> str:
        """
        This function extracts text from the specified file.
        """
        # Extract text from a temporary file
        text = FileUtil.extract_text_from_file(file_path)
        return text

    def check_file(file_path: Annotated[str, "File path"]) -> bool:
        """
        This function checks if the specified file exists.
        """
        # Check if the file exists
        import os
        check_file = os.path.exists(file_path)
        return check_file

    def extract_webpage(url: Annotated[str, "URL of the web page to extract text and links from"]) -> Annotated[tuple[str, list[tuple[str, str]]], "Page text, list of links (href attribute and link text from <a> tags)"]:
        """
        This function extracts text and links from the specified URL of a web page.
        """
        # ヘッドレスモードのオプションを設定
        edge_options = Options()
        edge_options.add_argument("--headless")
        edge_options.add_argument("--disable-gpu")
        edge_options.add_argument("--no-sandbox")
        edge_options.add_argument("--disable-dev-shm-usage")

        # Edgeドライバをセットアップ
        driver = webdriver.Edge(service=Service(EdgeChromiumDriverManager().install()), options=edge_options)
        # Wait for the page to fully load (set explicit wait conditions if needed)
        driver.implicitly_wait(10)
        # Retrieve HTML of the web page and extract text and links
        driver.get(url)
        # Get the entire HTML of the page
        page_html = driver.page_source

        from bs4 import BeautifulSoup
        soup = BeautifulSoup(page_html, "html.parser")
        text = soup.get_text()
        # Retrieve href attribute and text from <a> tags
        urls: list[tuple[str, str]] = [(a.get("href"), a.get_text()) for a in soup.find_all("a")]
        driver.close()
        return text, urls

    def search_duckduckgo(query: Annotated[str, "String to search for"], num_results: Annotated[int, "Maximum number of results to display"], site: Annotated[str, "Site to search within. Leave blank if no site is specified"] = "") -> Annotated[list[tuple[str, str, str]], "(Title, URL, Body) list"]:
        """
        This function searches DuckDuckGo with the specified keywords and returns related articles.
        ユーザーから特定のサイト内での検索を行うように指示を受けた場合、siteパラメータを使用して検索を行います。
        """
        
        from duckduckgo_search import DDGS
        ddgs = DDGS()
        try:
            # Retrieve DuckDuckGo search results
            if site:
                query = f"{query} site:{site}"

            print(f"Search query: {query}")

            results_dict = ddgs.text(
                keywords=query,            # Search keywords
                region='jp-jp',            # Region. For Japan: "jp-jp", No specific region: "wt-wt"
                safesearch='off',          # Safe search OFF->"off", ON->"on", Moderate->"moderate"
                timelimit=None,            # Time limit. None for no limit, past day->"d", past week->"w", past month->"m", past year->"y"
                max_results=num_results    # Number of results to retrieve
            )

            results = []
            for result in results_dict:
                # title, href, body
                title = result.get("title", "")
                href = result.get("href", "")
                body = result.get("body", "")
                print(f'Title: {title}, URL: {href}, Body: {body[:100]}')
                results.append((title, href, body))

            return results
        except Exception as e:
            print(e)
            import traceback
            traceback.print_exc()
            return []

    def save_text_file(name: Annotated[str, "File name"], dirname: Annotated[str, "Directory name"], text: Annotated[str, "Text data to save"]) -> Annotated[bool, "Save result"]:
        """
        This function saves text data as a file.
        """
        
        # Save in the specified directory
        try:
            import os
            if not os.path.exists(dirname):
                os.makedirs(dirname)
            path = os.path.join(dirname, name)
            with open(path, "w", encoding="utf-8") as f:
                f.write(text)
            # Check if the file exists
            return os.path.exists(path)
        except Exception as e:
            print(e)
            return False

    def save_tools(name: Annotated[str, "Function name"], description: Annotated[str, "Function description"], code: Annotated[str, "Function code"], dirname: Annotated[str, "Directory name for saving"]) -> Annotated[bool, "Save result"]:
        """
        This function saves Python code as a JSON file for AutoGen tools.
        """
        
        # Generate JSON string
        func_dict = {
            "name": name,
            "description": description,
            "content": code
        }
        # Save in the specified directory
        try:
            import os
            if not os.path.exists(dirname):
                os.makedirs(dirname)
            filename = os.path.join(dirname, f"{name}.json")
            with open(filename, "w", encoding="utf-8") as f:
                json.dump(func_dict, f, ensure_ascii=False, indent=4)
            return True
        except Exception as e:
            print(e)
            return False

    def get_current_time() -> str:
        """
        This function returns the current time in the format yyyy/mm/dd (Day) hh:mm:ss.
        """
        from datetime import datetime
        now = datetime.now()
        return now.strftime("%Y/%m/%d (%a) %H:%M:%S")

    return [
        search_wikipedia_ja, list_files_in_directory, extract_file, extract_webpage, 
        save_tools, check_file, save_text_file, search_duckduckgo, get_current_time
    ]

def create_vector_db_tools(autogen_props: AutoGenProps, vector_db_props_list: list[VectorDBProps]) -> list[callable]:
    def vector_search(query: Annotated[str, "String to search for"]) -> list[str]:
        """
        This function performs a vector search on the specified text and returns the related documents.
        """
        params: VectorSearchParameter = VectorSearchParameter(autogen_props.openai_props, vector_db_props_list, query)
        result = LangChainVectorDB.vector_search(params)
        # Retrieve documents from result
        documents = result.get("documents", [])
        # Extract content of each document from documents
        result = [doc.get("content", "") for doc in documents]
        return result

    return [vector_search]