namespace SistemaTarjetas.Models
{
    public class Cliente
    {
        public string Id { get; set; }
        public string Identificacion { get; set; }
        public string Nombre { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public string Correo { get; set; }
        public string Usuario { get; set; }
        public string Contrasena { get; set; }
        public string Estado { get; set; }
        public int Tipo { get; set; }
    }
}