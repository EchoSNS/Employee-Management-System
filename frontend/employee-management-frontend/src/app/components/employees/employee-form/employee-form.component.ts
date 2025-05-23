import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSelectModule } from '@angular/material/select';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { EmployeeService } from '../../../services/employee.service';
import { Employee, CreateEmployeeRequest, UpdateEmployeeRequest } from '../../../models/employee';

@Component({
  selector: 'app-employee-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatSelectModule,
    MatToolbarModule,
    MatIconModule,
    MatSnackBarModule,
    MatProgressSpinnerModule,
    MatCheckboxModule
  ],
  templateUrl: './employee-form.component.html',
  styleUrl: './employee-form.component.css'
})
export class EmployeeFormComponent implements OnInit {
  employeeForm: FormGroup;
  isEditMode: boolean = false;
  employeeId?: number;
  loading: boolean = false;
  submitting: boolean = false;

  departments: string[] = [
    'IT',
    'Human Resources',
    'Finance',
    'Marketing',
    'Sales',
    'Operations',
    'Engineering',
    'Customer Service',
    'Legal',
    'Administration'
  ];

  constructor(
    private fb: FormBuilder,
    private employeeService: EmployeeService,
    private router: Router,
    private route: ActivatedRoute,
    private snackBar: MatSnackBar
  ) {
    this.employeeForm = this.createForm();
  }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      if (params['id'] && params['id'] !== 'new') {
        this.isEditMode = true;
        this.employeeId = +params['id'];
        this.loadEmployee();
      }
    });
  }

  private createForm(): FormGroup {
    return this.fb.group({
      firstName: ['', [Validators.required, Validators.maxLength(100)]],
      lastName: ['', [Validators.required, Validators.maxLength(100)]],
      email: ['', [Validators.required, Validators.email, Validators.maxLength(255)]],
      phoneNumber: ['', [Validators.maxLength(20)]],
      department: ['', [Validators.required]],
      position: ['', [Validators.required, Validators.maxLength(100)]],
      salary: [0, [Validators.required, Validators.min(0.01)]],
      hireDate: ['', [Validators.required]],
      isActive: [true]
    });
  }

  private loadEmployee(): void {
    if (!this.employeeId) return;

    this.loading = true;
    this.employeeService.getEmployeeById(this.employeeId).subscribe({
      next: (employee) => {
        this.populateForm(employee);
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading employee:', error);
        this.snackBar.open('Error loading employee: ' + error, 'Close', {
          duration: 5000,
          panelClass: ['error-snackbar']
        });
        this.loading = false;
        this.goBack();
      }
    });
  }

  private populateForm(employee: Employee): void {
    this.employeeForm.patchValue({
      firstName: employee.firstName,
      lastName: employee.lastName,
      email: employee.email,
      phoneNumber: employee.phoneNumber || '',
      department: employee.department,
      position: employee.position,
      salary: employee.salary,
      hireDate: employee.hireDate,
      isActive: employee.isActive
    });
  }

  onSubmit(): void {
    if (this.employeeForm.invalid) {
      this.markFormGroupTouched();
      return;
    }

    this.submitting = true;
    const formValue = this.employeeForm.value;

    if (this.isEditMode && this.employeeId) {
      this.updateEmployee(formValue);
    } else {
      this.createEmployee(formValue);
    }
  }

  private createEmployee(formValue: any): void {
    const createRequest: CreateEmployeeRequest = {
      firstName: formValue.firstName,
      lastName: formValue.lastName,
      email: formValue.email,
      phoneNumber: formValue.phoneNumber || undefined,
      department: formValue.department,
      position: formValue.position,
      salary: formValue.salary,
      hireDate: formValue.hireDate
    };

    this.employeeService.createEmployee(createRequest).subscribe({
      next: (employee) => {
        this.snackBar.open('Employee created successfully!', 'Close', {
          duration: 3000,
          panelClass: ['success-snackbar']
        });
        this.router.navigate(['/employees', employee.id]);
      },
      error: (error) => {
        console.error('Error creating employee:', error);
        this.snackBar.open('Error creating employee: ' + error, 'Close', {
          duration: 5000,
          panelClass: ['error-snackbar']
        });
        this.submitting = false;
      }
    });
  }

  private updateEmployee(formValue: any): void {
    if (!this.employeeId) return;

    const updateRequest: UpdateEmployeeRequest = {
      firstName: formValue.firstName,
      lastName: formValue.lastName,
      email: formValue.email,
      phoneNumber: formValue.phoneNumber || undefined,
      department: formValue.department,
      position: formValue.position,
      salary: formValue.salary,
      hireDate: formValue.hireDate,
      isActive: formValue.isActive
    };

    this.employeeService.updateEmployee(this.employeeId, updateRequest).subscribe({
      next: (employee) => {
        this.snackBar.open('Employee updated successfully!', 'Close', {
          duration: 3000,
          panelClass: ['success-snackbar']
        });
        this.router.navigate(['/employees', employee.id]);
      },
      error: (error) => {
        console.error('Error updating employee:', error);
        this.snackBar.open('Error updating employee: ' + error, 'Close', {
          duration: 5000,
          panelClass: ['error-snackbar']
        });
        this.submitting = false;
      }
    });
  }

  private markFormGroupTouched(): void {
    Object.keys(this.employeeForm.controls).forEach(key => {
      const control = this.employeeForm.get(key);
      control?.markAsTouched();
    });
  }

  goBack(): void {
    this.router.navigate(['/employees']);
  }
}