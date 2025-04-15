import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NhanxetdinhkiComponent } from './nhanxetdinhki.component';

describe('NhanxetdinhkiComponent', () => {
  let component: NhanxetdinhkiComponent;
  let fixture: ComponentFixture<NhanxetdinhkiComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ NhanxetdinhkiComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(NhanxetdinhkiComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
