import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateloainhansuComponent } from './createloainhansu.component';

describe('CreateloainhansuComponent', () => {
  let component: CreateloainhansuComponent;
  let fixture: ComponentFixture<CreateloainhansuComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CreateloainhansuComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateloainhansuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
