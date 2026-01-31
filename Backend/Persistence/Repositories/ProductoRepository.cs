using Microsoft.Data.Sqlite;
using Backend.Models;
using Backend.Persistence.Context;

namespace Backend.Persistence.Repositories{
    public class ProductoRepository:IProductoRepository{
        private readonly ConexionSql _contexto;

        public ProductoRepository(ConexionSql contexto){
        _contexto = contexto;
    }

        public List<Producto> ObtenerTodos(){
            var productos = new List<Producto>();
            try{
                _contexto.Open();
                var command = _contexto.ObtenerConexion().CreateCommand();
                command.CommandText = "SELECT Id, Nombre, PrecioUnitario FROM Productos";
                
                using var reader = command.ExecuteReader();
                while (reader.Read()){
                    productos.Add(new Producto {
                        Id = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        PrecioUnitario = reader.GetDecimal(2)
                    });
                }
            }
            finally { _contexto.Close(); }
            return productos;
        }

        public int Crear(Producto producto){
            try{
                _contexto.Open();
                var command = _contexto.ObtenerConexion().CreateCommand();
                command.CommandText = @"
                    INSERT INTO Productos (Nombre, PrecioUnitario) 
                    VALUES (@nom, @precio);
                    SELECT last_insert_rowid();";
                
                command.Parameters.AddWithValue("@nom", producto.Nombre);
                command.Parameters.AddWithValue("@precio", producto.PrecioUnitario);

                return Convert.ToInt32(command.ExecuteScalar());
            }
            finally { _contexto.Close(); }
        }
    }
}