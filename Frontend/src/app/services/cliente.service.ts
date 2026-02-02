import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { HttpParams } from '@angular/common/http'; // Importante, esto permite llevar parámetros en las URLs sirve para el controlador GET
import { environment } from '../../environments/environment';
import { Cliente } from '../models/cliente.interface';

@Injectable({
  providedIn: 'root',
})
export class ClienteService {  
  private apiUrl = `${environment.apiUrl}/Cliente`;

  constructor(private http: HttpClient) { }

  getTodos(estado?: string, identificacion?: string): Observable<Cliente[]> {
    let params = new HttpParams();
    if (estado) params = params.set('estado', estado);
    if (identificacion) params = params.set('identificacion', identificacion);
    return this.http.get<Cliente[]>(this.apiUrl, { params });
  }
  crear(cliente: Cliente): Observable<Cliente> {
    return this.http.post<Cliente>(this.apiUrl, cliente);
  }
  actualizar(cliente: Cliente): Observable<Cliente> {
    return this.http.put<Cliente>(`${this.apiUrl}/${cliente.id}`, cliente);
  }
  eliminar(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}
