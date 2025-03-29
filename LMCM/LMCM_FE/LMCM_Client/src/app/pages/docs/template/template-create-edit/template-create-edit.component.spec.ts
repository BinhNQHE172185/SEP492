import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TemplateCreateEditComponent } from './template-create-edit.component';

describe('TemplateCreateEditComponent', () => {
  let component: TemplateCreateEditComponent;
  let fixture: ComponentFixture<TemplateCreateEditComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TemplateCreateEditComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TemplateCreateEditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
