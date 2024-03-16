import json
from funzioni_validazione import validazione_input,verifica_codice_volo
from config import path_elimina_volo as path
from utilities import contatta_server

def elimina_volo():
    
    codice_volo = validazione_input("Inserisci il codice del volo (es: CT1234): ", verifica_codice_volo, "codice volo non valido")
    _, risposta = contatta_server(path, json.dumps(codice_volo.upper()))

    print("\n" + risposta)

