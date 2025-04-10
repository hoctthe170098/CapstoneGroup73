import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChitietBaitapComponent } from './chitiet-baitap.component';

describe('ChitietBaitapComponent', () => {
  let component: ChitietBaitapComponent;
  let fixture: ComponentFixture<ChitietBaitapComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ChitietBaitapComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ChitietBaitapComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
