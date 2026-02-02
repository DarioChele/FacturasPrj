import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { Router } from '@angular/router'; 
import { FacturaService } from '../../services/factura.service';
import { Factura } from '../../models/factura.interface';
import { AuthService } from '../../services/auth.service';
@Component({
  selector: 'app-facturas',
  standalone: false,
  templateUrl: './facturas.html',
  styleUrl: './facturas.css',
})
export class Facturas implements OnInit {
  // 1. Definimos las propiedades que usará el HTML
  listaFacturas: Factura[] = [];
  mostrarModal = false;
  //facturaTmp: any = {};
  facturaTmp: Factura = {} as Factura;
  // Variables para los filtros
  filtroNumero: string = '';
  filtroFecha: string = '';
  filtroMonto?: number;

  // 2. EL CONSTRUCTOR: Aquí es donde "nace" FacturaService
  constructor(private router: Router,
              private facturaService: FacturaService, 
              public auth: AuthService, 
              private cdr: ChangeDetectorRef
            ) {}

  // 3. ngOnInit: Se ejecuta cuando la página carga
  ngOnInit(): void {
    this.cargarFacturas();
  }

  // 4. FUNCIÓN cargarFacturas
  cargarFacturas() {
    //this.facturaService.getTodos().subscribe({
    this.facturaService.getTodos(this.filtroNumero, this.filtroFecha, this.filtroMonto).subscribe({
      next: (res) => {        
        this.listaFacturas = res; // <--- ESTO es lo que llena la tabla
        this.cdr.detectChanges();
      },
      error: (err: any) => { // Le ponemos ': any' para que TypeScript no se queje xD
        console.error('Error al traer facturas', err);
      }
    });
  }

  nuevoFactura() {
    // Redirigimos a la ruta que creaste
    this.router.navigate(['/nueva-factura']);
  }

  imprimir() {
    window.print();
  }
  limpiarFiltros() {    
    this.filtroNumero = '';
    this.filtroFecha = '';
    this.filtroMonto = undefined;
    this.cargarFacturas(); // Recarga la lista completa
  }

  abrirModal(f: Factura) {
    this.facturaTmp = { ...f };
    this.mostrarModal = true;
  }  
  borrar(id: number | undefined) {
    if (!this.auth.esAdmin()) {
      alert('No tienes permisos para eliminar facturas.');
      return;
    }   
  }
}