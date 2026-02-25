import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { HttpParams } from '@angular/common/http'; // Importante, esto permite llevar parámetros en las URLs sirve para el controlador GET
import { environment } from '../../environments/environment';
import { Proveedor } from '../models/proveedor.interface';


@Injectable({
  providedIn: 'root',
})
export class ProveedorService {
  private apiUrl = `${environment.apiUrl}/Cliente`;

  constructor(private http: HttpClient) { }

    getTodos(estado?: string, identificacion?: string): Observable<Proveedor[]> {
      let params = new HttpParams();
      if (estado) params = params.set('estado', estado);
      if (identificacion) params = params.set('identificacion', identificacion);
      return this.http.get<Proveedor[]>(this.apiUrl, { params });
    }
    crear(proveedor: Proveedor): Observable<Proveedor> {
      return this.http.post<Proveedor>(this.apiUrl, proveedor);
    }
    actualizar(proveedor: Proveedor): Observable<Proveedor> {
      return this.http.put<Proveedor>(`${this.apiUrl}/${proveedor.id}`, proveedor);
    }
    eliminar(id: number): Observable<any> {
      return this.http.delete(`${this.apiUrl}/${id}`);
    }




  
}
