from utilities import stampa_menu
from funzioni_admin.inserisci_volo import inserisci_volo
from funzioni_admin.visualizza_volo import visualizza_volo
from funzioni_admin.elimina_volo import elimina_volo
from funzioni_admin.elimina_ricorrenza import elimina_ricorrenza
from funzioni_admin.inserisci_ricorrenza import inserisci_ricorrenza
from funzioni_admin.modifica_data_ricorrenza import modifica_data_ricorrenza


def routine_admin():

    opzione = 0
    while opzione != 8:
        stampa_menu(opzioni_menu_admin)
        opzione = input("Scelta: ")
        try:
            opzione = int(opzione)
            if opzione == 1:
                inserisci_volo()
            elif opzione == 2:
                visualizza_volo()
            elif opzione == 3:
                elimina_volo()
            elif opzione == 4:
                inserisci_ricorrenza()
            elif opzione == 5:
                elimina_ricorrenza()
            elif opzione == 6:
                modifica_data_ricorrenza()
            elif opzione == 7:
                from main import main  # Per evitare un errore di importazione circolare
                main()
            elif opzione == 8:
                print("Grazie per aver utilizzato Airline Manager! Arrivederci!")
                exit()
            else:
                raise ValueError
        except ValueError:
            print("\nHai inserito un'opzione non valida. Riprova.")


opzioni_menu_admin = [
                    "\n\n*** MENU ADMIN ***\n",
                    "1. Inserisci una nuova tratta",
                    "2. Visualizza ricorrenze di una tratta",
                    "3. Elimina tratta",
                    "4. Inserisci una nuova ricorrenza",
                    "5. Elimina una ricorrenza",
                    "6. Modifica la data di una ricorrenza",
                    "7. Torna al menu principale",
                    "8. Esci da Air-Manager\n"
                    ]