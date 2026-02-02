import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { HttpParams } from '@angular/common/http'; // Importante, esto permite llevar parámetros en las URLs sirve para el controlador GET
import { environment } from '../../environments/environment';
import { Factura } from '../models/factura.interface';
import { FormaPago } from '../models/forma-pago.interface';

@Injectable({
  providedIn: 'root',
})
export class FacturaService {
  private apiUrl = `${environment.apiUrl}/Factura`; 

  constructor(private http: HttpClient) { }

  getTodos(numero?: string, fecha?: string, monto?: number): Observable<Factura[]> {
    let params = new HttpParams();

    // Si el valor existe, lo añadimos a la consulta
    if (numero) params = params.set('numero', numero);
    if (fecha) params = params.set('fecha', fecha);
    if (monto) params = params.set('monto', monto.toString());

    return this.http.get<Factura[]>(this.apiUrl, { params });
    }

  crear(factura: Factura): Observable<Factura> {
    return this.http.post<Factura>(this.apiUrl, factura);
  }
  getFormasPago(): Observable<FormaPago[]> {
    return this.http.get<FormaPago[]>(`${environment.apiUrl}/FormasPago`);
  }
//   getById(factura: Factura): Observable<Factura> {
//     return this.http.get<Factura>(`${this.apiUrl}/${factura.id}`);
//   }
}