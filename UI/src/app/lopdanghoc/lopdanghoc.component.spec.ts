import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LopdanghocComponent } from './lopdanghoc.component';

describe('LopdanghocComponent', () => {
  let component: LopdanghocComponent;
  let fixture: ComponentFixture<LopdanghocComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LopdanghocComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LopdanghocComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
