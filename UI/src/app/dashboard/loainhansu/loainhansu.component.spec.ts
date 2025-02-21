import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LoainhansuComponent } from './loainhansu.component';

describe('LoainhansuComponent', () => {
  let component: LoainhansuComponent;
  let fixture: ComponentFixture<LoainhansuComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LoainhansuComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LoainhansuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
