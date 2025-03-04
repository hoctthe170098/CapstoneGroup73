import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CosoComponent } from './coso.component';

describe('CosoComponent', () => {
  let component: CosoComponent;
  let fixture: ComponentFixture<CosoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CosoComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CosoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
