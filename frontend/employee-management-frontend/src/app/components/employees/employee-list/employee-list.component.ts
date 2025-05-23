import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCardModule } from '@angular/material/card';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { animate, style, transition, trigger } from '@angular/animations';
import { EmployeeService } from '../../../services/employee.service';
import { EmployeeListItem } from '../../../models/employee';
import { ConfirmDialogComponent } from '../../shared/dialogs/confirm-dialog/confirm-dialog.component';
import { MatDividerModule } from '@angular/material/divider';

@Component({
  selector: 'app-employee-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatCardModule,
    MatToolbarModule,
    MatSnackBarModule,
    MatProgressSpinnerModule,
    MatDividerModule,
  ],
  templateUrl: './employee-list.component.html',
  styleUrls: ['./employee-list.component.css'],
  animations: [
    trigger('listAnimation', [
      transition(':enter', [
        style({ opacity: 0, transform: 'translateY(-20px)' }),
        animate('300ms ease-out', 
          style({ opacity: 1, transform: 'translateY(0)' }))
      ])
    ])
  ]
})
export class EmployeeListComponent implements OnInit {
  employees: EmployeeListItem[] = [];
  filteredEmployees: EmployeeListItem[] = [];
  searchTerm: string = '';
  loading: boolean = false;
  displayedColumns: string[] = ['fullName', 'email', 'department', 'position', 'status', 'actions'];

  constructor(
    private employeeService: EmployeeService,
    private router: Router,
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.loadEmployees();
  }

  loadEmployees(): void {
    this.loading = true;
    this.employeeService.getAllEmployees().subscribe({
      next: (employees) => {
        this.employees = employees;
        this.filteredEmployees = employees;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading employees:', error);
        this.snackBar.open('Error loading employees: ' + error, 'Close', {
          duration: 5000,
          panelClass: ['error-snackbar']
        });
        this.loading = false;
      }
    });
  }

  applyFilter(): void {
    if (!this.searchTerm.trim()) {
      this.filteredEmployees = this.employees;
      return;
    }

    const searchTermLower = this.searchTerm.toLowerCase().trim();
    this.filteredEmployees = this.employees.filter(employee =>
      employee.fullName.toLowerCase().includes(searchTermLower) ||
      employee.email.toLowerCase().includes(searchTermLower) ||
      employee.department.toLowerCase().includes(searchTermLower) ||
      employee.position.toLowerCase().includes(searchTermLower)
    );
  }

  addEmployee(): void {
    this.router.navigate(['/employees/new']);
  }

  viewEmployee(id: number): void {
    this.router.navigate(['/employees', id]);
  }

  editEmployee(id: number): void {
    this.router.navigate(['/employees', id, 'edit']);
  }

  deleteEmployee(id: number, employeeName: string): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '400px',
      data: {
        title: 'Delete Employee',
        message: `Are you sure you want to delete ${employeeName}? This action cannot be undone.`,
        confirmText: 'Delete',
        cancelText: 'Cancel'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.employeeService.deleteEmployee(id).subscribe({
          next: () => {
            this.snackBar.open(`${employeeName} has been deleted successfully`, 'Close', {
              duration: 3000,
              panelClass: ['success-snackbar']
            });
            this.loadEmployees(); // Refresh the list
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
}