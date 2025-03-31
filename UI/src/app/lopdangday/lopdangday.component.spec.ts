import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LopdangdayComponent } from './lopdangday.component';

describe('LopdangdayComponent', () => {
  let component: LopdangdayComponent;
  let fixture: ComponentFixture<LopdangdayComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LopdangdayComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LopdangdayComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
