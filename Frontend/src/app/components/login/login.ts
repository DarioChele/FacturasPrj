import { Component } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
// Estas variables se conectan al HTML
  userData = { User: '', Pwd: '' };
  errorMsg = '';
  constructor(private auth: AuthService, private router: Router) { }
  
  onSubmit() {
    this.auth.login(this.userData.User, this.userData.Pwd).subscribe({
      next: (res) => {
        this.router.navigate(['/clientes']); // Irá a esta vista al entrar
      },
      error: (err) => {
        this.errorMsg = 'Usuario o contraseña incorrectos';
        console.error(err);
      }
    });
  }
}
