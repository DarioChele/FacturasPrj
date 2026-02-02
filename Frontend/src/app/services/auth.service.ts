import { Injectable } from '@angular/core';
import { jwtDecode } from 'jwt-decode';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private apiUrl = `${environment.apiUrl}/Auth`;

  constructor(private http: HttpClient) { }

  login(user: string, pwd: string): Observable<any> {
    // Usamos los nombres exactos de LoginDTO: User y Pwd
    return this.http.post<any>(`${this.apiUrl}/login`, { User: user, Pwd: pwd })
      .pipe(
        tap(res => {
          // Si el back responde con el token, lo guardamos
          if (res && res.token) {
            localStorage.setItem('token_factura', res.token);
          }
        })
      );
  }
  esAdmin(): boolean {
    const token = localStorage.getItem('token_factura');
    if (!token) return false;

    try {
      const decoded: any = jwtDecode(token);
      // Buscamos la clave corta "role" o la larga de Microsoft
      const role = decoded['role'] || decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
      // Convertimos a string y comparamos para evitar problemas de tipos
      // Retorna TRUE si es "0", FALSE en cualquier otro caso    
      return String(role) === '0';
    } catch (error) {
      console.error('Error decodificando token', error);
      return false;
    }
  }
  // Método útil para saber si estamos logueados
  isLoggedIn(): boolean {
    //return !!localStorage.getItem('token_factura');
    const token = localStorage.getItem('token_factura');
    return token !== null && token !== undefined && token !== '';
  }
  getDecodedToken(): any {
    const token = localStorage.getItem('token_factura');
    if (!token) return null;
    try {
      const decoded: any = jwtDecode(token);
      return decoded;
    } catch (error) {
      console.error('Error decodificando token', error);
      return error as string;
    }
  }
  logout() {
    localStorage.removeItem('token_factura');
  }
}