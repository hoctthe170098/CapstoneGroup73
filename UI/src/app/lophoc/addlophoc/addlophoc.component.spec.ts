import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddlophocComponent } from './addlophoc.component';

describe('AddlophocComponent', () => {
  let component: AddlophocComponent;
  let fixture: ComponentFixture<AddlophocComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddlophocComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AddlophocComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
