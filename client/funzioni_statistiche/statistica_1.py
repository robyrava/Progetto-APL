from funzioni_validazione import verifica_codice_volo, verifica_anno,validazione_input
from utilities import contatta_server
from config import path_statistica_uno as path
import json
import matplotlib.pyplot as plt

#numero di riccorrenze di un volo in un anno

def statistica_1():
    
    codice_volo = validazione_input("Inserisci il codice del volo (es: CT1234): ", verifica_codice_volo, "Codice volo non valido")
    anno = validazione_input("Inserisci l'anno: ", verifica_anno, "Anno non valido")
        
    # dizionario
    dati = {"codice_volo": codice_volo.upper(), "anno": anno}
    
    #Invio dati al server
    _, risposta = contatta_server(path, json.dumps(dati),True)

    if risposta == "":
        print("\nNessun dato disponibile")
        return
    
    
    # Converto il risultato in una lista di tuple (settimana, elementi)
    result = [tuple(map(int, row.split())) for row in risposta.split("\n") if row]
    
    
    # Estraggo le settimane e gli elementi in due liste separate
    mese = [row[0] for row in result]
    elementi = [row[1] for row in result]

    # Creo un dizionario per mappare le settimane ai mesi
    mese_map = {1: "Gennaio", 2: "Febbraio", 3: "Marzo", 4: "Aprile", 5: "Maggio", 6: "Giugno", 7: "Luglio", 8: "Agosto", 9: "Settembre", 10: "Ottobre", 11: "Novembre", 12: "Dicembre"}

    # Converto le settimane nei nomi dei mesi corrispondenti
    mese = [mese_map[row[0]] for row in result]

    # Creo un grafico ad istogramma con i mesi sull'asse x e gli elementi sull'asse y
    plt.bar(mese, elementi, width=0.1)

    # valori stampati asse x
    plt.xticks(rotation='vertical')

    # titolo negli assi
    plt.title(f"Ricorrenze volo {codice_volo.upper()} nell'anno {anno}")
    plt.ylabel("Num. Voli")

    # Mostra il grafico
    plt.show()

  