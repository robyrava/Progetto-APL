using System.Text.RegularExpressions;
using Classi; // aggiunto per poter utilizzare le classi Volo e Ricorrenza

namespace Funzionalità
{
    class Validator
    {
        static string regex;

//RR97--------------------------------------------------------------------------------------------------------------

        //metodo che verifica se la data inserita sia nel formato AAAA-MM-GG e che sia una data valida (es. 2020-02-30 non è una data valida)
        public static bool VerificaData(string data)
        {
            // Verifica il formato della data
            regex = @"^(\d{4})-(\d{2})-(\d{2})$";
            Match match = Regex.Match(data, regex);
            if (!match.Success)
            {
                return false;
            }

            // Estrae anno, mese e giorno
            int anno = int.Parse(match.Groups[1].Value);
            int mese = int.Parse(match.Groups[2].Value);
            int giorno = int.Parse(match.Groups[3].Value);

            // Verifica la validità del mese e del giorno
            try
            {
                DateTime dataInserita = new DateTime(anno, mese, giorno);
            }
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }

            return true;
        }

        // se dataCorrente è precedente a inizioPeriodoValido
        // if dataCorrente <= finePeriodoValido
        // if finePeriodoValido >= dataCorrente

        

        //metodo che confronta:
        //1) data 1 < data odierna se boolean = false e data 2 = null
        //2) data 1 <= data odierna se boolean = true e data 2 = null
        //3) data 2 >= data 1 se boolean = true e data 2 != null
        public static bool ConfrontaData(string data1, string? data2 = null, bool boolean = false)
        {
            //Verifico formato data
            if (!VerificaData(data1))
            {
                return false;
            }
            if (data2 != null)
            {
                if (!VerificaData(data2))
                {
                    return false;
                }
            }

            // Converto la stringa in un oggetto DateTime
            int anno, mese, giorno;
            string[] parti = data1.Split('-');
            anno = int.Parse(parti[0]);
            mese = int.Parse(parti[1]);
            giorno = int.Parse(parti[2]);
            DateTime dataInserita = new DateTime(anno, mese, giorno);
            DateTime dataCorrente = DateTime.Today; // data corrente

            if (boolean)  // true = confronto data di arrivo/partenza ; confronto data di partenza/data odierna
            {
                if (data2 != null) //verifico se data arrivo è maggiore o uguale a data partenza
                {
                    parti = data2.Split('-');
                    anno = int.Parse(parti[0]);
                    mese = int.Parse(parti[1]);
                    giorno = int.Parse(parti[2]);
                    dataCorrente = new DateTime(anno, mese, giorno);
                }
                if (dataInserita < dataCorrente)
                {
                    return false;
                }
            }
            else  // false = data nascita/data odierna
            {
                if (dataInserita >= dataCorrente)
                {
                    return false;
                }

            }
            return true;
        }

        // metodo di validazione codice volo
        public static bool VerificaCodiceVolo(string codiceVolo)
        {
            regex = "^[A-Za-z]{2}\\d{4}$";

            if (!Regex.IsMatch(codiceVolo, regex))
            {
                Console.WriteLine("Codice volo: formato NON valido");
                return false;
            }

            return true;
        }


        // metodo per verificare il prezzo base
        public static bool VerificaPrezzoBase(string prezzoBase)
        {
            regex = @"^(9999|([1-9][0-9]{0,3})(?:[.,][0-9]{1,2})?|0(?:[.,][0-9]{2})?)$";

            if (!Regex.Match(prezzoBase, regex).Success)
            {
                Console.WriteLine("Prezzo base non valido");
                return false;
            }
            return true;
        }


        public static bool VerificaVoloCompleto(Volo volo)
        {
            //verifico codice volo con l'opportuno metodo
            if (!VerificaCodiceVolo(volo.CodiceVolo))
            {
                return false;
            }

            //verifico iata Aeroporto Partenza e Arrivo
            if (!VerificaIata(volo.IataAeroportoPartenza, volo.IataAeroportoArrivo))
            {
                return false;
            }

            //verifico prezzo base
            if (!VerificaPrezzoBase(volo.PrezzoBase.ToString()))
            {
                return false;
            }

            return true;
        }


        //Verifica che ID admin sia composto da 4 cifre    
        private static bool VerificaIdAdmin(int id)
        {
            regex = "^[0-9]{4}$";

            if (!Regex.Match(id.ToString(), regex).Success)
            {
                return false;
            }

            return true;
        }


        private static bool VerificaPasswordAdmin(string password)
        {
            regex = "^[A-Za-z0-9]{6}$";

            if (!Regex.Match(password, regex).Success)
            {
                return false;
            }

            return true;
        }


        public static bool VerificaLoginAdmin(Admin admin)
        {
            //verifico id
            if (!VerificaIdAdmin(admin.id))
            {
                return false;
            }

            //verifico password
            if (!VerificaPasswordAdmin(admin.password))
            {
                return false;
            }

            return true;
        }

//RR97 & GV----------------------------------------------------------------------------------------------------------

        // metodo di validazione codice iata
        public static bool VerificaIata(string iataPartenza, string? iataArrivo = null)
        {
            regex = "^[A-Za-z]{3}$";

            if (iataArrivo != null)
            {
                if (!Regex.Match(iataPartenza, regex).Success || !Regex.Match(iataArrivo, regex).Success)
                {
                    Console.WriteLine("IATA Aeroporto partenza e/o arrivo non valido/i");
                    return false;
                }

                return true;
            }

            if (!Regex.Match(iataPartenza, regex).Success)
            {
                Console.WriteLine("IATA Aeroporto partenza non valido");
                return false;
            }

            return true;
        }

//GV--------------------------------------------------------------------------------------------------------------

        // metodo di validazione importo prenotazione
        public static bool VerificaImporto(float importo)
        {
            if (importo > 0)
            {
                Console.WriteLine("Importo: formato valido (numero positivo)");
                return true;
            }
            else
            {
                Console.WriteLine("Importo: formato NON valido");
                return false;
            }
        }


        // metodo di validazione codice prenotazione
        public static bool VerificaCodicePrenotazione(string codicePrenotazione)
        {
            regex = "^[A-Za-z0-9]{12}$";

            if (Regex.IsMatch(codicePrenotazione, regex))
            {
                Console.WriteLine("Codice prenotazione: formato valido");
                return true;
            }
            else
            {
                Console.WriteLine("Codice prenotazione: formato NON valido");
                return false;
            }
        }


        // metodo di validazione documento
        public static bool VerificaDocumento(string documento)
        {
            regex = "^[A-Za-z]{2}\\d{7}$";

            if (Regex.IsMatch(documento, regex))
            {
                Console.WriteLine("Documento: formato valido");
                return true;
            }
            else
            {
                Console.WriteLine("Documento: formato NON valido");
                return false;
            }
        }


        // metodo di validazione codice fiscale
        public static bool VerificaCodiceFiscale(string codiceFiscale)
        {
            regex = "^[A-Za-z]{6}\\d{2}[A-Za-z]\\d{2}[A-Za-z]\\d{3}[A-Za-z]$";

            if (Regex.IsMatch(codiceFiscale, regex))
            {
                Console.WriteLine("Codice fiscale: formato valido");
                return true;
            }
            else
            {
                Console.WriteLine("Codice fiscale: formato NON valido");
                return false;
            }
        }


        // metodo di validazione nome/cognome
        public static bool VerificaNomeCognome(string valore)
        {
            regex = "^[A-Za-z]{1,20}$";

            if (Regex.IsMatch(valore, regex))
            {
                Console.WriteLine("Nome/Cognome: formato valido");
                return true;
            }
            else
            {
                Console.WriteLine("Nome/Cognome: formato NON valido");
                return false;
            }
        }


        // metodo di validazione telefono
        public static bool VerificaTelefono(string valore)
        {
            regex = "^\\d{10}$";

            if (Regex.IsMatch(valore, regex))
            {
                Console.WriteLine("Telefono: formato valido");
                return true;
            }
            else
            {
                Console.WriteLine("Telefono:: formato NON valido");
                return false;
            }
        }    

    }
}
