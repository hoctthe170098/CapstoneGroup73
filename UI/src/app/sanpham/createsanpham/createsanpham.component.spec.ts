import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreatesanphamComponent } from './createsanpham.component';

describe('CreatesanphamComponent', () => {
  let component: CreatesanphamComponent;
  let fixture: ComponentFixture<CreatesanphamComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CreatesanphamComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CreatesanphamComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
