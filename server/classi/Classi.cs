namespace Classi
{
    public class Prenotazione
    {
        public string CodicePrenotazione { get; set; }
        public string Documento { get; set; }
        public string? CodiceVolo { get; set; }
        public string? DataPartenza { get; set; }
        public float Importo { get; set; }
    }
    public class Volo
    {
        public string CodiceVolo { get; set; }
        public string IataAeroportoPartenza { get; set; }
        public string IataAeroportoArrivo { get; set; }
        public string PrezzoBase { get; set; }

    }
    class Ricorrenza : Volo
    {
        public string DataPartenza { get; set; }
        public string DataArrivo { get; set; }
    }

    public class Passeggero
    {
        public string Documento { get; set; }
        public string CodicePrenotazione { get; set; }
        public string CodiceFiscale { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string DataNascita { get; set; }
        public string Telefono { get; set; }
    }

    public class Admin
    {
        public int id { get; set; }
        public string password { get; set; }
    }
}