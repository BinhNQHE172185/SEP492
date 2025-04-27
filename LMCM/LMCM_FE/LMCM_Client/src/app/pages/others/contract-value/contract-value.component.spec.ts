import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ContractValueComponent } from './contract-value.component';

describe('ContractValueComponent', () => {
  let component: ContractValueComponent;
  let fixture: ComponentFixture<ContractValueComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ContractValueComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ContractValueComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
