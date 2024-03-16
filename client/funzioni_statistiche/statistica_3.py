from utilities import contatta_server
from funzioni_validazione import validazione_input, verifica_anno
from config import path_statistica_tre as path
import matplotlib.pyplot as plt
import json

#

def statistica_3():

    anno = validazione_input("Inserisci l'anno: ", verifica_anno, "Anno non valido")
    
    # dizionario
    dati = {"anno": anno}
    # richiesta POST al server
    _, risposta = contatta_server(path, json.dumps(dati), True)

    if risposta == "":
        print("\nNessun dato disponibile")
        return
    
    # Converto il risultato in una lista di tuple (aeroporto, ricorrenze)
    result = [tuple(row.split()) for row in risposta.split("\n") if row]

    # Estraggo gli aeroporti e le ricorrenze in due liste separate
    aeroporti = [row[0] for row in result]
    ricorrenze = [int(row[1]) for row in result]

    # Creo un grafico
    plt.bar(aeroporti, ricorrenze, edgecolor="black", facecolor="orange", width=0.1)

    # titolo negli agli assi
    plt.title("Numero di ricorrenze per aeroporto di partenza nell'anno " + anno)
    plt.xlabel("Aeroporto")
    plt.ylabel("Ricorrenze")

    # Mostra il grafico
    plt.show()
