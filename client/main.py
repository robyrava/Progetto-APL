from funzioni_admin.admin import routine_admin
from funzioni_utente.utente import routine_utente
from funzioni_statistiche.statistiche import routine_statistiche
from utilities import *
import json
from funzioni_validazione import validazione_input,verifica_id_admin,verifica_password_admin
from config import *


def login_admin():
    print("\n***Login admin***\n")
    
    #INSERIMENTO NOME UTENTE E PASSWORD
    identificatore = validazione_input("Inserisci codice ID: ", verifica_id_admin, "codice ID non valido: deve essere formato da 4 numeri")
    password = validazione_input("Inserisci password: ", verifica_password_admin, "La password deve essere composta da 6 caratteri. Riprova.")

    #Creo il dizionario con i dati inseriti
    dati = {"id": int(identificatore), "password": str(password)}

    esito, risposta = contatta_server(path_login_admin, json.dumps(dati))  #attenzione esito è una tupla

    print("\n" + risposta)

    if(esito):
        routine_admin()
    else:
        main()


def main():
    stampa_menu(opzioni_menu_avvio)
    opzione = input("Scelta: ")

    while opzione != 4:
        try:
            opzione = int(opzione)
            if opzione == 1:
                routine_utente()
            elif opzione == 2:
                login_admin()
            elif opzione == 3:
                routine_statistiche()
            elif opzione == 4:
                print("\nGrazie per aver utilizzato Airline Manager! Arrivederci!")
                exit()
            else:
                raise ValueError
        except ValueError:
            print("Inserisci un'opzione valida")

        opzione = input("Scelta: ")


opzioni_menu_avvio = ["\nBenvenuto in Airline Manager\n", "1. Utente", "2. Admin", "3. Statistiche", "4. Esci\n"]

        
if __name__ == "__main__":
    main()

