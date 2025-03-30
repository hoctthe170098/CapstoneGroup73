import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountmanagerComponent } from './quanly.component';

describe('AccountmanagerComponent', () => {
  let component: AccountmanagerComponent;
  let fixture: ComponentFixture<AccountmanagerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AccountmanagerComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AccountmanagerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
