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
    
    public async Task OpenAsync() {
        if (_conexion.State != ConnectionState.Open)
            await _conexion.OpenAsync();
    }

    public void Open() {
        if (_conexion.State != ConnectionState.Open)
            _conexion.Open();
    }

    public void Close() {
        if (_conexion.State != ConnectionState.Closed)
            _conexion.Close();
    }

    // --- Transacciones Asíncronas ---
    public async Task InitTranAsync() {
        await OpenAsync();
        _transaccion = _conexion.BeginTransaction();
    }

    public async Task CommitTranAsync() {
        if (_transaccion != null) await _transaccion.CommitAsync();
    }

    public async Task RollBackAsync() {
        if (_transaccion != null) await _transaccion.RollbackAsync();
    }

    public SqliteConnection ObtenerConexion() => _conexion;

    public void Dispose() {
        _conexion.Close();
        _conexion.Dispose();
    }
}