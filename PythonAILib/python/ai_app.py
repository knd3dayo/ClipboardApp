import os, json
from PIL import Image
from io import StringIO
import tempfile
import sys
sys.path.append("python")

from openai_props import OpenAIProps, VectorDBProps
from openai_client import OpenAIClient
import langchain_util
import langchain_vector_db

import excel_util

# Proxy環境下でのSSLエラー対策。HTTPS_PROXYが設定されていない場合はNO_PROXYを設定する
if "HTTPS_PROXY" not in os.environ:
    os.environ["NO_PROXY"] = "*"

# stdout,stderrを文字列として取得するためラッパー関数を定義
def capture_stdout_stderr(func):
    def wrapper(*args, **kwargs):
        # strout,stderrorをStringIOでキャプチャする
        buffer = StringIO()
        sys.stdout = buffer
        sys.stderr = buffer
        try:
            result = func(*args, **kwargs)
        except Exception as e:
            # エラーが発生した場合はエラーメッセージを出力
            print(e)
            result = None
        # strout,stderrorを元に戻す
        sys.stdout = sys.__stdout__
        sys.stderr = sys.__stderr__
        
        # bufferをリストに変換して返す
        log = []
        for line in buffer.getvalue().splitlines():
            log.append(line)

        return result, log
    return wrapper

# ファイルからテキストを抽出する
def extract_text_from_file(filename):
    import file_extractor
    return file_extractor.extract_text_from_file(filename)

# base64形式のデータからテキストを抽出する
def extract_base64_to_text(base64_data):
    # base64データから一時ファイルを生成
    with tempfile.NamedTemporaryFile(delete=False) as temp:
        # base64からバイナリデータに変換
        base64_data = base64_data.encode()
        temp.write(base64_data)
        temp_path = temp.name
        temp.close()
        
        import file_extractor
        # 一時ファイルからテキストを抽出
        text = file_extractor.extract_text_from_file(temp_path)
        # 一時ファイルを削除
        os.remove(temp_path)
        return text

########################
# openai関連
########################
def run_openai_chat(props_json: str, request_json: str):
    # OpenAIチャットを実行する関数を定義
    def func() -> str:
        # OpenAIPorpsを生成
        props = json.loads(props_json)
        openai_props = OpenAIProps(props)
        # OpenAIClientを生成
        openai_client = OpenAIClient(openai_props)
        # request_jsonをdictに変換
        request = json.loads(request_json)
        content = openai_client.run_openai_chat(request)
        return content

    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行
    content, log = wrapper()

    # 結果格納用のdictを生成
    result = {}
    result["content"] = content
    # dict["log"]にログを追加
    result["log"] = log
    
    # resultをJSONに変換して返す
    result_json = json.dumps(result, ensure_ascii=False, indent=4)
    return result_json


def openai_embedding(props_json: str, input_text: str):
    # OpenAIPorpsを生成
    props = json.loads(props_json)
    openai_props = OpenAIProps(props)
    # OpenAIClientを生成
    openai_client = OpenAIClient(openai_props)
    
    return openai_client.openai_embedding(input_text)

def list_openai_models(props_json: str):
    props = json.loads(props_json)
    openai_props = OpenAIProps(props)
    client = OpenAIClient(openai_props)
    return client.list_openai_models()

########################
# langchain関連
########################

def run_vector_search(props_json: str, request_json: str):
    # OpenAIチャットを実行する関数を定義
    def func() -> dict:
        result = langchain_util.run_vector_search(props_json, request_json)
        return result
    
    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行
    result, log = wrapper()
    
    # resultにlogを追加
    result["log"] = log
    
    # resultをJSONに変換して返す
    result_json = json.dumps(result, ensure_ascii=False, indent=4)
    return result_json

def run_langchain_chat( props_json: str, request_prompt: str, request_json: str):
    # OpenAIチャットを実行する関数を定義
    def func() -> dict:

        # process_langchain_chat_parameterを実行
        openai_props, vector_db_props, prompt, chat_history_json  = langchain_util.process_langchain_chat_parameter(props_json, request_prompt, request_json)
        # langchan_chatを実行
        result = langchain_util.langchain_chat(openai_props, vector_db_props, prompt, chat_history_json)
        return result
    
    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行
    result, log = wrapper()
    
    # resultにlogを追加
    result["log"] = log
    
    # resultをJSONに変換して返す
    result_json = json.dumps(result, ensure_ascii=False, indent=4)
    return result_json

# vector db関連
def update_file_index(props_json, request_json):
    return __update_or_delete_file_index(props_json, request_json, "update")

def delete_file_index(props_json, request_json):
    return __update_or_delete_file_index(props_json, request_json, "delete")


def __update_or_delete_file_index(props_json, request_json, mode):

    # update_indexを実行する関数を定義
    def func () -> dict:
        # props_json, request_jsonからOpenAIProps, VectorDBProps, mode, workdir, relative_path, urlを取得
        openai_props, vector_db_props, document_root, relative_path, url, description = langchain_vector_db.process_file_update_or_datele_request_params(props_json, request_json)
        # LangChainVectorDBを生成
        vector_db = langchain_vector_db.get_vector_db(openai_props, vector_db_props)

        # modeに応じて処理を分岐
        if mode == "delete":
            # delete_file_indexを実行
            vector_db.delete_file_index(document_root, relative_path, url)
        if mode == "update":
            # update_file_indexを実行
            vector_db.update_file_index(document_root, relative_path, url, description=description)

        # 結果用のdictを生成
        result = {}
        return result
    
    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行
    result, log = wrapper()
    
    # resultにlogを追加
    result["log"] = log

    # resultをJSONに変換して返す
    result_json = json.dumps(result, ensure_ascii=False, indent=4)
    return result_json

# ベクトルDBのコンテンツインデックスを削除する
def delete_content_index(props_json, request_json):
    return __update_or_delete_content_index(props_json, request_json, "delete")

# ベクトルDBのコンテンツインデックスを更新する
def update_content_index(props_json, request_json):
    return __update_or_delete_content_index(props_json, request_json, "update")

def __update_or_delete_content_index(props_json, request_json, mode):
    # update_indexを実行する関数を定義
    def func () -> dict:
        # props_json, request_jsonからOpenAIProps, VectorDBProps, text, sourceを取得
        openai_props, vector_db_props, text, source, source_url, description  = langchain_vector_db.process_content_update_or_datele_request_params(props_json, request_json)

        # LangChainVectorDBを生成
        vector_db = langchain_vector_db.get_vector_db(openai_props, vector_db_props)
        
        if mode == "delete":
            # delete_content_indexを実行
            vector_db.delete_content_index(source)
        if mode == "update":
            # update_content_indexを実行
            vector_db.update_content_index(text, source, source_url, description=description)
            
        # 結果用のdictを生成
        result = {}
        return result

    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行
    result, log = wrapper()
    
    # resultにlogを追加
    result["log"] = log
    
    # resultをJSONに変換して返す
    result_json = json.dumps(result, ensure_ascii=False, indent=4)
    return result_json

# ベクトルDBの画像インデックスを削除する
def delete_image_index(props_json, request_json):
    return __update_or_delete_image_index(props_json, request_json, "delete")

# ベクトルDBの画像インデックスを更新する
def update_image_index(props_json, request_json):
    return __update_or_delete_image_index(props_json, request_json, "update")

def __update_or_delete_image_index(props_json, request_json, mode):
    # update_indexを実行する関数を定義
    def func () -> dict:
        # props_json, request_jsonからOpenAIProps, VectorDBProps, text, image_url, sourceを取得
        openai_props, vector_db_props, text, source, source_url, description, image_url = langchain_vector_db.process_image_update_or_datele_request_params(props_json, request_json)
        # LangChainVectorDBを生成
        vector_db = langchain_vector_db.get_vector_db(openai_props, vector_db_props)
        
        # 初期化
        update_count = 0
        delete_count = 0
        
        if mode == "delete":
            # delete_image_indexを実行
             vector_db.delete_image_index(source)
        if mode == "update":
            # update_image_indexを実行
            vector_db.update_image_index(text,  source, source_url, description=description, image_url=image_url)
            
        # 結果用のdictを生成
        result = {}
        return result
    
    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行
    result, log = wrapper()
    
    # resultにlogを追加
    result["log"] = log
    
    # resultをJSONに変換して返す
    result_json = json.dumps(result, ensure_ascii=False, indent=4)
    return result_json

# export_to_excelを実行する
def export_to_excel(filePath, dataJson):
    # export_to_excelを実行する関数を定義
    def func() -> dict:
        # dataJsonをdictに変換
        data = json.loads(dataJson)
        # export_to_excelを実行
        print(data)
        excel_util.export_to_excel(filePath, data.get("rows",[]))
        # 結果用のdictを生成
        result = {}
        return result
    
    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行
    result, log = wrapper()
    
    # resultにlogを追加
    result["log"] = log
    
    # resultをJSONに変換して返す
    result_json = json.dumps(result, ensure_ascii=False, indent=4)
    return

# import_from_excelを実行する
def import_from_excel(filePath):
    # import_to_excelを実行する関数を定義
    def func() -> dict:
        # import_to_excelを実行
        data = excel_util.import_from_excel(filePath)
        # 結果用のdictを生成
        result = {}
        result["rows"] = data
        return result
    
    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行
    result, log = wrapper()
    
    # resultにlogを追加
    result["log"] = log
    
    # resultをJSONに変換して返す
    result_json = json.dumps(result, ensure_ascii=False, indent=4)
    return result_json

# テスト用
def hello_world():
    return "Hello World"
