import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChitietBaitaphocsinhComponent } from './chitiet-baitaphocsinh.component';

describe('ChitietBaitaphocsinhComponent', () => {
  let component: ChitietBaitaphocsinhComponent;
  let fixture: ComponentFixture<ChitietBaitaphocsinhComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ChitietBaitaphocsinhComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ChitietBaitaphocsinhComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
