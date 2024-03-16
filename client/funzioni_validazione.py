import re
import datetime

#RR97---------------------------------------------------------------------------------------------------
def validazione_input(messaggio_input, funzione_validazione, messaggio_errore):
    while True:
        dato = input(messaggio_input)
        if funzione_validazione(dato):
            return dato
        print(messaggio_errore)

def verifica_mese(mese):
    regex = "^(0?[1-9]|1[0-2])$"

    if re.match(regex, mese) != None:
        return True
    
    return False

def verifica_anno(anno):
    regex = "^(202[0-9])$"

    if re.match(regex, anno) != None:
        return True
    
    return False

def verifica_posto(posto, elenco_posti):
    posto = posto.upper()
    #Verifico che il posto sia presente tra i posti disponibili e che sia impostato a False
    if posto in elenco_posti:
        if elenco_posti[posto] == "false":
            return True
    else:
        return False


def verifica_id_admin(id):
    regex = "^[0-9]{4}$"

    if re.match(regex, str(id)) != None:
        return True
    
    return False


def verifica_password_admin(password):
    regex = "^[A-Za-z0-9]{6}$"

    if re.match(regex, password) != None:
        return True
    
    return False
    

# funzione che verifica che la data inserita sia nel formato AAAA-MM-GG e che sia una data valida (es. 2020-02-30 non è una data valida)
def verifica_data(data):
    # Verifica il formato della data
    regex = r"^(\d{4})-(\d{2})-(\d{2})$"
    match = re.search(regex, data)
    if not match:
        return False

    # Estrae anno, mese e giorno
    anno, mese, giorno = map(int, match.groups())

    # Verifica la validità del mese e del giorno
    try:
        datetime.datetime(anno, mese, giorno)
    except ValueError:
        return False

    return True

#funzione che confronta:
    #1) data 1 < data odierna se boolean = false e data 2 = null
    #2) data 1 <= data odierna se boolean = true e data 2 = null
    #3) data 2 >= data 1 se boolean = true e data 2 != null
def confronta_data(data1, data2=None, boolean=False):
    # Verifico formato data
    if verifica_data(data1) != True:
        return False
    if data2 != None:
        if verifica_data(data2) != True:
            return False

    # Converto la stringa in un oggetto datetime
    anno, mese, giorno = map(int, data1.split('-'))
    data_inserita = datetime.date(anno, mese, giorno)
    data_corrente = datetime.date.today()  # data corrente

    if boolean:  # true = confronto data di arrivo/partenza ; confronto data di partenza/data odierna
        if data2 != None:  # verifica data di arrivo >= data di partenza
            anno, mese, giorno = map(int, data2.split('-'))
            data_corrente = datetime.date(anno, mese, giorno)
        if data_inserita < data_corrente:
            return False
    else:  # false = data nascita/data odierna
        if data_inserita >= data_corrente:
            return False

    return True

#verifico che il codice del volo sia composto da 6 caratteri con i primi due lettere e i restanti numeri
def verifica_codice_volo(codiceVolo):
    #regex = "^[A-Z]{2}\\d{4}$" # precedente
    regex = "^[A-Za-z]{2}\\d{4}$"

    if re.match(regex, codiceVolo) != None:
        return True
    else:
        return False


#verifico che il prezzo base inserito sia composto da numeri compresi tra 0 e 9999. Valori con la virgola ammessi.
def verifica_prezzo_base(prezzoBase):
    regex = "^(9999|([1-9][0-9]{0,3})(?:[.,][0-9]{1,2})?|0(?:[.,][0-9]{2})?)$"
    if re.match(regex, prezzoBase) != None:
        return True
    else:
        return False
#RR97, GV-------------------------------------------------------------------------------------

def verifica_iata(iata):
    regex = "^[A-Za-z]{3}$"

    if re.match(regex, iata) != None:
        return True
    else:
        return False

#GV -------------------------------------------------------------------------------------

def verifica_iata_diversi(iata_aeroporto_partenza, iata_aeroporto_arrivo):
    if iata_aeroporto_partenza.upper() == iata_aeroporto_arrivo.upper():
        return False
    else:
        return True


def verifica_documento(documento):
    regex = "^[A-Za-z]{2}\\d{7}$"

    if re.match(regex, documento) != None:
        return True
    else:
        return False

def verifica_codice_fiscale(codiceFiscale):
    regex = "^[A-Z]{6}\\d{2}[A-Z]\\d{2}[A-Z]\\d{3}[A-Z]$"

    if re.match(regex, codiceFiscale) != None:
        return True
    else:
        return False


# verifico che il nome/cognome abbia una dimensione compresa fra 1 e 20 e contenga solo lettere dell'alfabeto, sia maiuscole sia minuscole
def verifica_nome_cognome (valore):
    regex = "^[A-Za-z]{1,20}$"

    if re.match(regex, valore) != None:
        return True
    else:
        return False

def verifica_telefono(telefono):
    regex = "^\\d{10}$"

    if re.match(regex, telefono) != None:
        return True
    else:
        return False


def verifica_codice_prenotazione(codice_prenotazione):
    regex = "^[A-Za-z0-9]{12}$"

    if re.match(regex, codice_prenotazione) != None:
        return True
    else:
        return False