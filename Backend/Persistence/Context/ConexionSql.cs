using Microsoft.Data.Sqlite;
using System.Data;

namespace Backend.Persistence.Context;
public class ConexionSql : IDisposable {
    private readonly SqliteConnection _conexion;
    private SqliteTransaction? _transaccion; 
    private readonly IConfiguration _configuration;

    public ConexionSql(IConfiguration configuration) {
        _configuration = configuration;
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        _conexion = new SqliteConnection(connectionString);
    }

    public void Open() {
        if (_conexion.State != ConnectionState.Open)
            _conexion.Open();
    }

    public void Close() {
        if (_conexion.State != ConnectionState.Closed)
            _conexion.Close();
    }

    // Para las facturas: Iniciar una transacción
    public void InitTran() {
        Open();
        _transaccion = _conexion.BeginTransaction();
    }

    public void CommitTran() => _transaccion?.Commit();
    public void RollBack() => _transaccion?.Rollback();

    // Acceso a la conexión para los comandos del repositorio
    public SqliteConnection ObtenerConexion() => _conexion;

    public void Dispose() {
        _conexion.Close();
        _conexion.Dispose();
    }
}