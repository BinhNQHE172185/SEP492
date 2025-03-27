import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ContractCreateEditComponent } from './contract-create-edit.component';

describe('ContractCreateEditComponent', () => {
  let component: ContractCreateEditComponent;
  let fixture: ComponentFixture<ContractCreateEditComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ContractCreateEditComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ContractCreateEditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
