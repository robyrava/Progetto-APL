using System.Text;
using Classi;
using MySql.Data.MySqlClient;


namespace Funzionalità
{
    class InterrogazioniDB
    {
        static string query;

//GV----------------------------------------------------------------------------------------------------------------

        // ricerca voli per acquisto, sulla base dei criteri specificati
        public static MySqlDataReader? CercaVoliDataIata(Configurazione conf, MySqlConnection connection, Ricorrenza r)
        {
            string and;

            if (r.IataAeroportoArrivo == null) // si specifica solo iata partenza
            {
                and = "";
            }
            else // si specifica sia iata partenza sia iata arrivo
            {
                and = "AND v.iataAeroportoArrivo = @iataAeroportoArrivo ";
            }

            query = "SELECT v.codiceVolo, v.iataAeroportoPartenza, v.iataAeroportoArrivo, v.prezzoBase, r.dataPartenza, r.dataArrivo, r.numeroPostiOccupati " +
                    "FROM voli AS v " +
                    "INNER JOIN ricorrenze AS r " +
                    "ON v.codiceVolo = r.codiceVolo " +
                    "WHERE r.dataPartenza = @dataPartenza AND v.iataAeroportoPartenza = @iataAeroportoPartenza " + and + "AND numeroPostiOccupati < 90";

            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@dataPartenza", r.DataPartenza);
            cmd.Parameters.AddWithValue("@iataAeroportoPartenza", r.IataAeroportoPartenza.ToUpper());

            if (r.IataAeroportoArrivo != null)
            {
                cmd.Parameters.AddWithValue("@iataAeroportoArrivo", r.IataAeroportoArrivo.ToUpper());
            }

            cmd.Prepare();
            MySqlDataReader dataReader = cmd.ExecuteReader();

            if (dataReader.HasRows) // verifico che sia stata recuperata dal DB almeno una tupla
            {
                return dataReader;
            }

            return null;
        }


        // inserimento dati nuova prenotazione
        public static (bool, string?) InserisciPrenotazione(MySqlConnection connection, Prenotazione p)
        {
            p.CodicePrenotazione = GeneraCodicePrenotazione();

            query = "INSERT INTO prenotazioni(codicePrenotazione, documento, codiceVolo, dataPartenza, dataAcquisto, importo) VALUES(@codicePrenotazione, @documento, @codiceVolo, @dataPartenza, @dataAcquisto, @importo)";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@codicePrenotazione", p.CodicePrenotazione.ToUpper());
            cmd.Parameters.AddWithValue("@documento", p.Documento.ToUpper());
            cmd.Parameters.AddWithValue("@codiceVolo", p.CodiceVolo.ToUpper());
            cmd.Parameters.AddWithValue("@dataPartenza", p.DataPartenza);
            cmd.Parameters.AddWithValue("@dataAcquisto", DateTime.Today.Date);
            cmd.Parameters.AddWithValue("@importo", p.Importo);
            //cmd.Parameters.AddWithValue("@bagaglio", 0); // in base al setting del DB, viene fatto di default
            cmd.Prepare();

            if (cmd.ExecuteNonQuery() == 1) // verifico se l'operazione in DB è stata eseguita correttamente
            {
                return (true, p.CodicePrenotazione);
            }

            return (false, null);
        }


        // genera codice alfanumerico di 12 caratteri
        static string GeneraCodicePrenotazione()
        {
            const int length = 12;
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"; // set di caratteri da cui estrarre ciascun carattere
            StringBuilder sb = new StringBuilder(); // String non è mutabile, StringBuilder sì, quindi è più efficiente per costruire la stringa finale un carattere alla volta
            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                int index = random.Next(chars.Length); // restituisce un intero casuale non negativo inferiore al massimo specificato.
                sb.Append(chars[index]); // tramite l'index generato randomicamente, seleziono un carattere dal set di caratteri e lo aggiungo a sb
            }

            //Console.WriteLine($"Il codice alfanumerico generato è: {sb}");
            return sb.ToString(); //converte il valore dell'istanza sb di StringBuilder in un oggetto di tipo String
        }


        // inserimento dati nuovo passeggero
        public static bool InserisciPasseggero(MySqlConnection connection, Passeggero p)
        {
            query = "INSERT INTO passeggeri(documento, codicePrenotazione, codiceFiscale, nome, cognome, dataNascita, telefono) VALUES(@documento, @codicePrenotazione ,@codiceFiscale, @nome, @cognome, @dataNascita, @telefono)";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@documento", p.Documento.ToUpper());
            cmd.Parameters.AddWithValue("@codicePrenotazione", p.CodicePrenotazione.ToUpper());
            cmd.Parameters.AddWithValue("@codiceFiscale", p.CodiceFiscale.ToUpper());
            cmd.Parameters.AddWithValue("@nome", p.Nome);
            cmd.Parameters.AddWithValue("@cognome", p.Cognome);
            cmd.Parameters.AddWithValue("@dataNascita", p.DataNascita);
            cmd.Parameters.AddWithValue("@telefono", p.Telefono);
            cmd.Prepare();

            if (cmd.ExecuteNonQuery() == 1) // verifico se l'operazione in DB è stata eseguita correttamente
            {
                return true;
            }
            
            return false;
        }


        // inserimento dati aggiornati per una ricorrenza specificata
        public static bool ModificaRicorrenzaNumeroPostiOccupati(MySqlConnection connection, string codiceVolo, string dataPartenza, int valore)
        {
            query = "UPDATE ricorrenze SET numeroPostiOccupati = (SELECT numeroPostiOccupati from ricorrenze WHERE codiceVolo = @codiceVolo AND dataPartenza = @dataPartenza)+@valore " +
                    "WHERE codiceVolo = @codiceVolo AND dataPartenza = @dataPartenza";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@codiceVolo", codiceVolo.ToUpper());
            cmd.Parameters.AddWithValue("@dataPartenza", dataPartenza);
            cmd.Parameters.AddWithValue("@valore", valore);
            cmd.Prepare();

            if (cmd.ExecuteNonQuery() == 1) // verifico se l'operazione in DB è stata eseguita correttamente
            {
                return true;
            }
            
            return false;
        }


        // ricerca prenotazione sulla base di codice volo, data partenza
        public static (bool, MySqlDataReader?) CercaPrenotazione(MySqlConnection connection, string codicePrenotazione, string documento)
        {
            query = "SELECT codiceVolo, dataPartenza " +
                    "FROM prenotazioni " +
                    "WHERE codicePrenotazione = @codicePrenotazione AND documento = @documento AND dataPartenza > @dataOdierna";

            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@codicePrenotazione", codicePrenotazione.ToUpper());
            cmd.Parameters.AddWithValue("@documento", documento.ToUpper());
            cmd.Parameters.AddWithValue("@dataOdierna", DateTime.Today.Date);
            cmd.Prepare();
            MySqlDataReader dataReader = cmd.ExecuteReader();

            if (dataReader.HasRows) // può essere trovata in DB al più una prenotazione
            {
                return (true, dataReader);
            }
            
            return (false, null);
        }


        // ricerca carta d'imbarco sulla base del codice prenotazione
        public static bool CercaCartaImbarco(MySqlConnection connection, string codicePrenotazione)
        {
            query = "SELECT codicePrenotazione " +
                    "FROM carte_imbarco " +
                    "WHERE codicePrenotazione = @codicePrenotazione";

            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@codicePrenotazione", codicePrenotazione.ToUpper());
            cmd.Prepare();
            MySqlDataReader dataReader = cmd.ExecuteReader();

            bool exists = dataReader.HasRows;
            dataReader.Close();

            // restituisce: true, se esiste già una carta imbarco associata al codice prenotazione fornito; false altrimenti
            return exists; 
        }


        // recupero dati carta d'imbarco per "Visualizza carta d'imbarco"
        public static MySqlDataReader? RecuperaDatiCartaImbarco(MySqlConnection connection, string codicePrenotazione)
        {
            query = "SELECT p.bagaglio, ci.codiceBiglietto, ci.posto " +
                    "FROM carte_imbarco AS ci " +
                    "INNER JOIN prenotazioni AS p " +
                    "ON ci.codicePrenotazione = p.codicePrenotazione " +
                    "WHERE p.codicePrenotazione = @codicePrenotazione";

            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@codicePrenotazione", codicePrenotazione.ToUpper());
            cmd.Prepare();
            MySqlDataReader dataReader = cmd.ExecuteReader();

            if (dataReader.HasRows)
            {
                return dataReader;
            }

            return null;
        }


        // recupero elenco dei posti a sedere, per una ricorrenza specificata
        public static (bool, string?) SelezionaPosti(MySqlConnection connection, string codiceVolo, string dataPartenza)
        {
            query = "SELECT posti " +
                    "FROM ricorrenze " +
                    "WHERE codiceVolo = @codiceVolo AND dataPartenza = @dataPartenza";

            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@codiceVolo", codiceVolo.ToUpper());
            cmd.Parameters.AddWithValue("@dataPartenza", dataPartenza);
            cmd.Prepare();
            MySqlDataReader dataReader = cmd.ExecuteReader();

            if (dataReader.HasRows)
            {
                dataReader.Read();
                string posti = dataReader["posti"].ToString();
                dataReader.Close();
                return (true, posti);
            }
            return (false, null);
        }


        // inserimento dati nuova carta d'imbarco
        public static (bool, string?) InserisciCartaImbarco(MySqlConnection connection, Prenotazione p, string posto)
        {
            // genero codice biglietto (codice alfanumerico composto da 12 cifre: le prime 9 sono relative al documento, le restanti 3 sono numeri interi casuali
            Random random = new Random();
            string codiceBiglietto = random.Next(10) + random.Next(10) + p.Documento.Substring(0, 9) + random.Next(10);

            query = "INSERT INTO carte_imbarco(codicePrenotazione, codiceBiglietto, codiceVolo, documento, posto) " +
                    "VALUES(@codicePrenotazione, @codiceBiglietto, @codiceVolo, @documento, @posto)";
            
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@codicePrenotazione", p.CodicePrenotazione.ToUpper());
            cmd.Parameters.AddWithValue("@codiceBiglietto", codiceBiglietto.ToUpper());
            cmd.Parameters.AddWithValue("@codiceVolo", p.CodiceVolo.ToUpper());
            cmd.Parameters.AddWithValue("@documento", p.Documento.ToUpper());
            cmd.Parameters.AddWithValue("@posto", posto);
            cmd.Prepare();

            if (cmd.ExecuteNonQuery() == 1) // verifico se l'operazione in DB è stata eseguita correttamente
            {
                return (true, codiceBiglietto);
            }

            return (false, null);
        }


        // eliminazione prenotazione specificata
        public static bool EliminaPrenotazione(MySqlConnection connection, string codicePrenotazione, string documento)
        {
            query = "DELETE FROM prenotazioni " +
                    "WHERE codicePrenotazione = @codicePrenotazione AND documento = @documento";

            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@codicePrenotazione", codicePrenotazione.ToUpper());
            cmd.Parameters.AddWithValue("@documento", documento.ToUpper());
            cmd.Prepare();

            if (cmd.ExecuteNonQuery() == 1) // verifico se l'operazione in DB è stata eseguita correttamente
            {
                return true;
            }

            return false;
        }


        // eliminazione passeggero specificato
        public static bool EliminaPasseggero(MySqlConnection connection, string codicePrenotazione, string documento)
        {
            query = "DELETE FROM passeggeri " +
                    "WHERE codicePrenotazione = @codicePrenotazione AND documento = @documento";

            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@codicePrenotazione", codicePrenotazione.ToUpper());
            cmd.Parameters.AddWithValue("@documento", documento.ToUpper());
            cmd.Prepare();

            if (cmd.ExecuteNonQuery() == 1) // verifico se l'operazione in DB è stata eseguita correttamente
            {
                return true;
            }

            return false;
        }


//RR97--------------------------------------------------------------------------------------------------------------
        
        public static bool LoginAdmin(MySqlConnection connection, Admin admin)
        {
            query = "SELECT * FROM admin WHERE id = @id AND password = @password";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@id", admin.id);
            cmd.Parameters.AddWithValue("@password", admin.password);
            cmd.Prepare();

            Console.WriteLine(admin.id + " " + admin.password  );

            MySqlDataReader r = cmd.ExecuteReader();
            bool ok = r.HasRows;
            r.Close();

            return ok;
        }
        
        // bool occupato:
        //      true: quando si vuole occupare un posto
        //      false: quando si vuole liberare un posto
        public static bool ModificaRicorrenzaPosti(MySqlConnection connection, string codiceVolo, string dataPartenza, string posto, string occupato)
        {
            query = "UPDATE ricorrenze SET posti = JSON_REPLACE(posti, '$[0].\"" + posto + "\"'," + "\"" + occupato + "\") WHERE codiceVolo = @codiceVolo AND dataPartenza = @dataPartenza";

            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@codiceVolo", codiceVolo.ToUpper());
            cmd.Parameters.AddWithValue("@dataPartenza", dataPartenza);
            cmd.Prepare();

            if (cmd.ExecuteNonQuery() == 1)
            {
                return true;
            }

            return false;
        }
        public static void AggiornaPostoCartaImbarco(MySqlConnection connection, string codicePrenotazione, string documento, string posto)
        {
            query = "UPDATE carte_imbarco SET posto = @posto WHERE codicePrenotazione = @codicePrenotazione AND documento = @documento";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@codicePrenotazione", codicePrenotazione.ToUpper());
            cmd.Parameters.AddWithValue("@documento", documento.ToUpper());
            cmd.Parameters.AddWithValue("@posto", posto.ToUpper());
            cmd.Prepare();

            cmd.ExecuteNonQuery();
        }
        public static string? RecuperaPosto(MySqlConnection connection, string codicePrenotazione, string documento)
        {
            query = "SELECT posto FROM carte_imbarco WHERE codicePrenotazione = @codicePrenotazione AND documento = @documento";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@codicePrenotazione", codicePrenotazione.ToUpper());
            cmd.Parameters.AddWithValue("@documento", documento.ToUpper());
            cmd.Prepare();

            MySqlDataReader r = cmd.ExecuteReader();
            r.Read();
            string? posto = r["posto"].ToString();
            r.Close();

            return posto;
        }
    

        //se inserisci = false ==> verifica se il bagaglio è stato già inserito;
        //   - Ritorna True se il bagaglio non è presente nel DB 
        //se inserisci = true ==> inserisco il bagaglio
        public static bool GestioneBagaglio(MySqlConnection connection, string codicePrenotazione, string documento, bool inserisci = false )
        {
            if (inserisci)
                query = "UPDATE prenotazioni SET bagaglio = 1 WHERE codicePrenotazione = @codicePrenotazione AND documento = @documento";
            else
                query = "SELECT * FROM PRENOTAZIONI WHERE codicePrenotazione = @codicePrenotazione AND documento = @documento AND bagaglio = 0";

            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@codicePrenotazione", codicePrenotazione.ToUpper());
            cmd.Parameters.AddWithValue("@documento", documento.ToUpper());
            cmd.Prepare();

            //return cmd.ExecuteReader().HasRows;
            MySqlDataReader r = cmd.ExecuteReader();
            bool ok = r.HasRows;
            r.Close();

            return ok;
        }

        //Ritorna il numero delle righe inserite nel db
        public static int InserisciVolo(MySqlConnection connection, Volo volo)
        {
            query = "INSERT INTO VOLI (codiceVolo, iataAeroportoPartenza, iataAeroportoArrivo, prezzoBase) SELECT @codiceVolo, @iataAeroportoPartenza, @iataAeroportoArrivo, @prezzoBase WHERE NOT EXISTS (SELECT 1 FROM VOLI WHERE codiceVolo = @codiceVolo)";
            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@codiceVolo", volo.CodiceVolo.ToUpper());
            command.Parameters.AddWithValue("@iataAeroportoPartenza", volo.IataAeroportoPartenza.ToUpper());
            command.Parameters.AddWithValue("@iataAeroportoArrivo", volo.IataAeroportoArrivo.ToUpper());
            command.Parameters.AddWithValue("@prezzoBase",  float.Parse(volo.PrezzoBase));
            command.Prepare();

            return command.ExecuteNonQuery();
        }

        //ritorna true se il volo è presente nel DB sennò false
        public static MySqlDataReader CercaVolo(MySqlConnection connection, string codiceVolo)
        {
            query = "SELECT v.iataAeroportoPartenza, v.iataAeroportoArrivo, v.prezzoBase, r.dataPartenza, r.dataArrivo, r.numeroPostiOccupati FROM voli v JOIN ricorrenze r ON v.codiceVolo = r.codiceVolo WHERE v.codiceVolo = @codiceVolo";
            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@codiceVolo", codiceVolo);
            command.Prepare();

            return command.ExecuteReader();        
        }

        public static int EliminaVolo(MySqlConnection connection, string codiceVolo)
        {
            query = "DELETE FROM voli WHERE codiceVolo = @codiceVolo";
            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@codiceVolo", codiceVolo);
            command.Prepare();

            return command.ExecuteNonQuery();
        }

        //Ritorna il numero delle righe inserite nel db
        public static int InserisciRicorrenza(MySqlConnection connection, Ricorrenza ricorrenza)
        {
            query = "INSERT INTO RICORRENZE (codiceVolo, dataPartenza, dataArrivo, posti) SELECT @codiceVolo, @dataPartenza, @dataArrivo, @posti FROM VOLI WHERE codiceVolo = @codiceVolo AND NOT EXISTS (SELECT * FROM RICORRENZE WHERE codiceVolo = @codiceVolo AND dataPartenza = @dataPartenza)";
            
            string posti;
            
            //Leggo l'elenco dei posti dal file json
            try
            {
                
                posti = File.ReadAllText("json/Posti.json");
            }
            catch (FileNotFoundException ex)
            {
                // Handle the exception here (e.g., log the error, show an error message)
                Console.WriteLine("Errore durante la lettura del file json: " + ex.Message);
                posti = string.Empty;
            }


            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@codiceVolo", ricorrenza.CodiceVolo.ToUpper());
            command.Parameters.AddWithValue("@dataPartenza", ricorrenza.DataPartenza);
            command.Parameters.AddWithValue("@dataArrivo", ricorrenza.DataArrivo);
            command.Parameters.AddWithValue("@posti", posti);
            command.Prepare();
            
            return command.ExecuteNonQuery();
        }

        public static int EliminaRicorrenza(MySqlConnection connection, Ricorrenza ricorrenza)
        {
            query = "DELETE r FROM RICORRENZE r JOIN VOLI v ON r.codiceVolo = v.codiceVolo WHERE r.codiceVolo = @codiceVolo AND r.dataPartenza = @dataPartenza AND r.dataArrivo = @dataArrivo";
            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@codiceVolo", ricorrenza.CodiceVolo.ToUpper());
            command.Parameters.AddWithValue("@dataPartenza", ricorrenza.DataPartenza);
            command.Parameters.AddWithValue("@dataArrivo", ricorrenza.DataArrivo);
            command.Prepare();
            
            return command.ExecuteNonQuery();
        }   

        public static int ModificaRicorrenza(MySqlConnection connection, Dictionary<string, string> ricorrenza)
        {
            query = "UPDATE RICORRENZE SET dataPartenza = @nuova_dataPartenza, dataArrivo = @nuova_dataArrivo WHERE codiceVolo = @codiceVolo AND dataPartenza = @dataPartenza AND dataArrivo = @dataArrivo";
            
            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@codiceVolo", ricorrenza["codiceVolo"].ToUpper());
            command.Parameters.AddWithValue("@dataPartenza", ricorrenza["dataPartenza"]);
            command.Parameters.AddWithValue("@dataArrivo", ricorrenza["dataArrivo"]);
            command.Parameters.AddWithValue("@nuova_dataPartenza", ricorrenza["nuovaDataPartenza"]);
            command.Parameters.AddWithValue("@nuova_dataArrivo", ricorrenza["nuovaDataArrivo"]);
            command.Prepare();

            return command.ExecuteNonQuery();
        }

//--------------------------------------------------------------------------------------------------------------

    }
}
