import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DashboardCMComponent } from './dashboard-cm.component';

describe('DashboardCMComponent', () => {
  let component: DashboardCMComponent;
  let fixture: ComponentFixture<DashboardCMComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DashboardCMComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DashboardCMComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
