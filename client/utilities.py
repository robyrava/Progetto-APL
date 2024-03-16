import requests, time
from config import URL,URL_STATISTICHE

#RR97--------------------------------------------------------------------------------------------------------------
def gestione_errori(funzione):
    def wrapper(*args, **kwargs):
        try:
            return funzione(*args, **kwargs)
        except requests.exceptions.RequestException as e:
            print("\nErrore durante la richiesta al server:", e)
            print("\nChiusura in corso...")
            exit()
    return wrapper

@gestione_errori 
def contatta_server(path, dati, statistica = False ):
    if statistica:
        r = requests.post(URL_STATISTICHE + path, dati)
    else:
        r = requests.post(URL + path, dati)

    if r.status_code == 200:
        return True, r.text
    else:
        return False, r.text

#GV---------------------------------------------------------------------

def stampa_menu(strings):
    for string in strings:
        print(string)

def simula_tempo_attesa(stringa_1, stringa_2=None):
    print(stringa_1)
    time.sleep(4)
    if(stringa_2 != None):
        print(stringa_2)


def stampa_voli_trovati(risultati):

    print("\nEcco qui i voli corrispondenti ai parametri di ricerca da te inseriti. \nEffettua una scelta inserendo il numero corrispondente: \n")
    i = 1
    for risultato in risultati:
        print("\t\t- Volo " + str(i) + ": ")
        print("\t\tcodice volo: " + risultato['codiceVolo'])
        print("\t\taeroporto di partenza: " + risultato['iataAeroportoPartenza'])
        print("\t\taeroporto di arrivo: " + risultato['iataAeroportoArrivo'])
        print("\t\tdata di partenza: " + risultato['dataPartenza'][0:10])
        print("\t\tdata di arrivo: " + risultato['dataArrivo'][0:10])
        print("\t\tprezzo: €" + str(risultato['prezzoBase']) + "\n")
        i = i + 1

    return i


def genera_dizionario_passeggero(documento, codice_prenotazione, codice_fiscale, nome, cognome, data_nascita, telefono):
    dati = {
        "documento": documento,
        "codicePrenotazione": codice_prenotazione,
        "codiceFiscale": codice_fiscale,
        "nome": nome,
        "cognome": cognome,
        "dataNascita": data_nascita,
        "telefono": telefono
    }

    return dati


def stampa_riepilogo_prenotazione(codice_prenotazione, volo_selezionato):

    print("\nRiepilogo prenotazione: ")

    data_partenza = formatta_data(volo_selezionato)
    print("codice prenotazione: " + codice_prenotazione.replace("\n", ""))
    print("codice volo: " + volo_selezionato['codiceVolo'])
    print("tratta: " +  volo_selezionato['iataAeroportoPartenza'] + " -> " + volo_selezionato['iataAeroportoArrivo'])
    print("data di partenza: " + data_partenza)
    print("importo: €" + str(volo_selezionato['prezzoBase']))
    print("\nGrazie per averci scelto, ti aspettiamo a bordo!\n")


def stampa_carta_imbarco(risposta, visualizza=False):
    print("\nRiepilogo carta d'imbarco: ")
    print("Codice prenotazione: " + risposta['codicePrenotazione'])
    print("Codice biglietto: " + risposta['codiceBiglietto'])
    print("Codice volo: " + risposta['codiceVolo'])
    if visualizza:
        data_partenza = formatta_data(risposta)
        print("Data partenza: " + data_partenza)
    print("Documento: " + risposta['documento'])
    print("Posto assegnato: " + risposta['postoAssegnato'])
    if visualizza:
        bagaglio = risposta['bagaglio']
        if bagaglio:
            output = "SI"
        else:
            output = "NO"
        print("Bagaglio: " + output + "\n")


def formatta_data(dizionario):
    anno_mese_giorno = dizionario['dataPartenza'][0:10].split('-')
    data_partenza = anno_mese_giorno[2] + "-" + anno_mese_giorno[1] + "-" + anno_mese_giorno[0]
    return data_partenza


def genera_labels_mesi(mesi):

    lista_mesi = []

    for mese in mesi: # mesi = [1,2,3,...,12]
        lista_mesi.append(dizionario_mesi[mese])


dizionario_mesi = {
        1: "GEN",
        2: "FEB",
        3: "MAR",
        4: "APR",
        5: "MAG",
        6: "GIU",
        7: "LUG",
        8: "AGO",
        9: "SET",
        10: "OTT",
        11: "NOV",
        12: "DIC",
    }