import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdatekehoachComponent } from './updatekehoach.component';

describe('UpdatekehoachComponent', () => {
  let component: UpdatekehoachComponent;
  let fixture: ComponentFixture<UpdatekehoachComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UpdatekehoachComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(UpdatekehoachComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
