import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NuevaFactura } from './nueva-factura';

describe('NuevaFactura', () => {
  let component: NuevaFactura;
  let fixture: ComponentFixture<NuevaFactura>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [NuevaFactura]
    })
    .compileComponents();

    fixture = TestBed.createComponent(NuevaFactura);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
