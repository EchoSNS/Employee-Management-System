import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDividerModule } from '@angular/material/divider';
import { MatChipsModule } from '@angular/material/chips';
import { MatDialog } from '@angular/material/dialog';
import { EmployeeService } from '../../../services/employee.service';
import { Employee } from '../../../models/employee';
import { ConfirmDialogComponent } from '../../shared/dialogs/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-employee-detail',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatToolbarModule,
    MatSnackBarModule,
    MatProgressSpinnerModule,
    MatDividerModule,
    MatChipsModule
  ],
  templateUrl: './employee-detail.component.html',
  styleUrl: './employee-detail.component.css'
})
export class EmployeeDetailComponent implements OnInit {
  employee?: Employee;
  loading: boolean = false;
  employeeId?: number;

  constructor(
    private employeeService: EmployeeService,
    private router: Router,
    private route: ActivatedRoute,
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      if (params['id']) {
        this.employeeId = +params['id'];
        this.loadEmployee();
      }
    });
  }

  private loadEmployee(): void {
    if (!this.employeeId) return;

    this.loading = true;
    this.employeeService.getEmployeeById(this.employeeId).subscribe({
      next: (employee) => {
        this.employee = employee;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading employee:', error);
        this.snackBar.open('Error loading employee: ' + error, 'Close', {
          duration: 5000,
          panelClass: ['error-snackbar']
        });
        this.loading = false;
      }
    });
  }

  editEmployee(): void {
    if (this.employeeId) {
      this.router.navigate(['/employees', this.employeeId, 'edit']);
    }
  }

  deleteEmployee(): void {
    if (!this.employee) return;

    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '400px',
      data: {
        title: 'Delete Employee',
        message: `Are you sure you want to delete ${this.employee.fullName}? This action cannot be undone.`,
        confirmText: 'Delete',
        cancelText: 'Cancel'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result && this.employeeId) {
        this.employeeService.deleteEmployee(this.employeeId).subscribe({
          next: () => {
            this.snackBar.open(`${this.employee?.fullName} has been deleted successfully`, 'Close', {
              duration: 3000,
              panelClass: ['success-snackbar']
            });
            this.router.navigate(['/employees']);
          },
          error: (error) => {
            console.error('Error deleting employee:', error);
            this.snackBar.open('Error deleting employee: ' + error, 'Close', {
              duration: 5000,
              panelClass: ['error-snackbar']
            });
          }
        });
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/employees']);
  }
}