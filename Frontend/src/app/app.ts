import { Component, signal } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.html',
  standalone: false,
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('Frontend');  
  // 2. Inyectar el Router en el constructor
  constructor(
    private router: Router,
    public auth: AuthService // Lo ponemos public para usarlo en el HTML
  ) {}

  cerrarSesion() {
    localStorage.removeItem('token_factura');
    this.router.navigate(['/login']); // 3. ¡Ahora sí funcionará!
  }
}
