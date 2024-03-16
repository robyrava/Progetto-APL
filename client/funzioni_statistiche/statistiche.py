from utilities import stampa_menu
from funzioni_statistiche.statistica_1 import statistica_1
from funzioni_statistiche.statistica_2 import statistica_2
from funzioni_statistiche.statistica_3 import statistica_3
from funzioni_statistiche.statistica_4 import statistica_4


def routine_statistiche():

    opzione = 0
    while opzione != 6:
        stampa_menu(opzioni_menu_statistiche)
        opzione = input("Scelta: ")
        try:
            opzione = int(opzione)
            if opzione == 1:
                statistica_1()
            elif opzione == 2:
                statistica_2()
            elif opzione == 3:
                statistica_3()
            elif opzione == 4:
                statistica_4()
            elif opzione == 5:
                from main import main  # Per evitare un errore di import circolare
                main()
            elif opzione == 6:
                print("Grazie per aver utilizzato Airline Manager! Arrivederci!")
                exit()
            else:
                raise ValueError
        except ValueError:
            print("\nHai inserito un'opzione non valida. Riprova.")


opzioni_menu_statistiche = [
                            "\n\n*** MENU STATISTICHE ***\n",
                            "1. Statistica 1: numero di ricorrenze di un volo in un determinato anno",
                            "2. Statistica 2: percentuale dei posti venduti rispetto al totale, per ciascuna ricorrenza di un volo",
                            "3. Statistica 3: numero di ricorrenze per aeroporto di partenza in un determinato anno",
                            "4. Statistica 4: guadagno mensile della compagnia, relativamente ad uno specifico anno",
                            "5. Torna al menu principale",
                            "6. Esci da Air-Manager\n"
                            ]
