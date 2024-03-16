from funzioni_utente.check_in import check_in
from funzioni_utente.acquista_biglietto import routine_acquista_biglietto
from funzioni_utente.modifica_posto import modifica_posto
from funzioni_utente.aggiungi_bagaglio import aggiungi_bagaglio
from funzioni_utente.cancella_prenotazione import cancella_prenotazione
from funzioni_utente.visualizza_carta_imbarco import visualizza_carta_imbarco
from utilities import stampa_menu


def routine_utente():

    opzione = 0
    while opzione != 8:
        stampa_menu(opzioni_menu_utente)
        opzione = input("Scelta: ")
        try:
            opzione = int(opzione)
            if opzione == 1:
                routine_acquista_biglietto()
            elif opzione == 2:
                modifica_posto()
            elif opzione == 3:
                aggiungi_bagaglio()
            elif opzione == 4:
                check_in()
            elif opzione == 5:
                cancella_prenotazione()
            elif opzione == 6:
                visualizza_carta_imbarco()
            elif opzione == 7:
                from main import main
                main()
            elif opzione == 8:
                print("Grazie per aver utilizzato Airline Manager! Arrivederci!")
                exit()
            else:
                raise ValueError
        except ValueError:
            print("\nHai inserito un'opzione non valida. Riprova.")


opzioni_menu_utente = [
                           "\n\n*** MENU UTENTE ***\n",
                           "1. Effettuare una prenotazione (Acquisto biglietto)",
                           "2. Modificare un posto assegnato (Modifica prenotazione)",
                           "3. Aggiungere un bagaglio (Modifica prenotazione)",
                           "4. Effettuare il check-in",
                           "5. Cancellare una prenotazione",
                           "6. Visualizza carta d'imbarco",
                           "7. Torna al menu principale",
                           "8. Esci da Airline-Manager\n"
                        ]