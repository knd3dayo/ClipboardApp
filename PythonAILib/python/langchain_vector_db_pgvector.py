
from ast import Tuple
from calendar import c
from importlib import metadata
import os, json, sys, uuid
from pydoc import doc
from telnetlib import DO
from langchain_community.callbacks import get_openai_callback

from langchain_postgres import PGVector
from langchain_postgres.vectorstores import PGVector
from sqlalchemy.orm import Session
from sqlalchemy import create_engine
from sqlalchemy.sql import text

from langchain_community.vectorstores.pgembedding import EmbeddingStore

sys.path.append("python")
from langchain_client import LangChainOpenAIClient
from langchain_vector_db import LangChainVectorDB
from openai_props import VectorDBProps

class LangChainVectorDBPGVector(LangChainVectorDB):

    def __init__(self, langchain_openai_client: LangChainOpenAIClient, vector_db_props: VectorDBProps):
        super().__init__(langchain_openai_client, vector_db_props)


    def _load(self):
        # VectorDBTypeStringが"PGVector"でない場合は例外をスロー
        if self.vector_db_props.VectorDBTypeString != "PGVector":
            raise ValueError("VectorDBTypeString must be 'PGVector'")

        # params
        params = {}
        params["connection"] = self.vector_db_props.VectorDBURL
        params["embeddings"] = self.langchain_openai_client.get_embedding_client()
        params["use_jsonb"] = True
        
        # collectionが指定されている場合
        print("CollectionName:", self.vector_db_props.CollectionName)
        if self.vector_db_props.CollectionName:
            params["collection_name"] = self.vector_db_props.CollectionName
        
        self.db = PGVector(
            **params
            )

    def _get_document_ids_by_tag(self, name:str=None, value:str=None) -> Tuple(list, list):
        engine = create_engine(self.vector_db_props.VectorDBURL)
        with Session(engine) as session:
            stmt = text("select uuid from langchain_pg_collection where name=:name")
            stmt = stmt.bindparams(name=self.vector_db_props.CollectionName)
            rows  = session.execute(stmt).fetchone()
            if (len(rows) == 0):
                return []
            collection_id = rows[0]
            print(collection_id)
            stmt = text("select id, cmetadata from langchain_pg_embedding where collection_id=:collection_id and cmetadata->>:name=:value")
            stmt = stmt.bindparams(collection_id=collection_id, name=name, value=value)
            rows = session.execute(stmt).all()
            document_ids = [row[0] for row in rows]
            metadata_list = [row[1] for row in rows]
            
            return document_ids, metadata_list

    def _save(self, documents:list=[]):
        self.db.add_documents(documents)
        
 
    def _delete(self, doc_ids:list=[]):
        if len(doc_ids) == 0:
            return

        self.db.delete(ids=doc_ids)

        return len(doc_ids)    
 
