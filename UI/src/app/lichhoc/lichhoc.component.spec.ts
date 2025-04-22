import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LichhocComponent } from './lichhoc.component';

describe('LichhocComponent', () => {
  let component: LichhocComponent;
  let fixture: ComponentFixture<LichhocComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LichhocComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LichhocComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
