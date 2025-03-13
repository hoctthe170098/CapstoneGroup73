import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LophocComponent } from './lophoc.component';

describe('LophocComponent', () => {
  let component: LophocComponent;
  let fixture: ComponentFixture<LophocComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LophocComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LophocComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
