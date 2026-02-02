import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { UsuarioService } from '../../services/usuario.service';
import { Usuario } from '../../models/usuario.interface';
import { AuthService } from '../../services/auth.service';
@Component({
  selector: 'app-usuarios',
  standalone: false,
  templateUrl: './usuarios.html',
  styleUrl: './usuarios.css',
})
export class Usuarios implements OnInit {
  // 1. Definimos las propiedades que usará el HTML
  listaUsuarios: Usuario[] = [];
  mostrarModal = false;
  //usuarioTmp: any = {};
  usuarioTmp: Usuario = {} as Usuario;

  // 2. EL CONSTRUCTOR: Aquí es donde "nace" UsuarioService
  constructor(
              private usuarioService: UsuarioService, 
              public auth: AuthService, 
              private cdr: ChangeDetectorRef
            ) {}

  // 3. ngOnInit: Se ejecuta cuando la página carga
  ngOnInit(): void {
    this.cargarUsuarios();
  }

  // 4. FUNCIÓN cargarUsuarios
  cargarUsuarios() {
    this.usuarioService.getTodos().subscribe({
      next: (res) => {        
        this.listaUsuarios = res; // <--- ESTO es lo que llena la tabla
        this.cdr.detectChanges();
      },
      error: (err: any) => { // Le ponemos ': any' para que TypeScript no se queje xD
        console.error('Error al traer usuarios', err);
      }
    });
  }

  nuevoUsuario() {
    this.usuarioTmp = {} as Usuario;
    this.mostrarModal = true;
  }
  abrirModal(usuario?: Usuario) {
    if (usuario) {
      // EDITAR: Creamos una copia para no modificar la tabla antes de guardar
      this.usuarioTmp = { ...usuario };
    } else {
      // NUEVO: Objeto limpio
      this.usuarioTmp = {} as Usuario;
    }
    this.mostrarModal = true;
  }
  
  guardar() {
    if (this.usuarioTmp.id) {
      // Si tiene ID, actualizamos
      this.usuarioService.actualizar(this.usuarioTmp).subscribe({
        next: () => this.finalizarGuardado('Usuario actualizado'),
        error: (err: any) => alert(err.message)
      });
    } else {
      // Si no tiene ID, creamos
      this.usuarioService.crear(this.usuarioTmp).subscribe({
        next: () => this.finalizarGuardado('Usuario creado'),
        error: (err: any) => alert(err.message)
      });
    }
  }
  finalizarGuardado(mensaje: string) {
    alert(mensaje);
    this.mostrarModal = false;
    this.cargarUsuarios();
  }
  borrar(id: number | undefined) {
    if (!this.auth.esAdmin()) {
      alert('No tienes permisos para eliminar usuarios.');
      return;
    }

    if (id && confirm('¿Estás seguro de eliminar este usuario?')) {
      this.usuarioService.eliminar(id).subscribe(() => this.cargarUsuarios());
    }
  }
}