import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { 
  Employee, 
  EmployeeListItem, 
  CreateEmployeeRequest, 
  UpdateEmployeeRequest 
} from '../models/employee';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {
  private readonly apiUrl = `${environment.apiUrl}/api/employees`;

  constructor(private http: HttpClient) {}

  getAllEmployees(): Observable<EmployeeListItem[]> {
    return this.http.get<EmployeeListItem[]>(this.apiUrl)
      .pipe(
        catchError(this.handleError)
      );
  }

  getEmployeeById(id: number): Observable<Employee> {
    return this.http.get<Employee>(`${this.apiUrl}/${id}`)
      .pipe(
        map(employee => ({
          ...employee,
          hireDate: new Date(employee.hireDate),
          createdAt: new Date(employee.createdAt),
          updatedAt: employee.updatedAt ? new Date(employee.updatedAt) : undefined
        })),
        catchError(this.handleError)
      );
  }

  createEmployee(employee: CreateEmployeeRequest): Observable<Employee> {
    return this.http.post<Employee>(this.apiUrl, employee)
      .pipe(
        map(createdEmployee => ({
          ...createdEmployee,
          hireDate: new Date(createdEmployee.hireDate),
          createdAt: new Date(createdEmployee.createdAt),
          updatedAt: createdEmployee.updatedAt ? new Date(createdEmployee.updatedAt) : undefined
        })),
        catchError(this.handleError)
      );
  }

  updateEmployee(id: number, employee: UpdateEmployeeRequest): Observable<Employee> {
    return this.http.put<Employee>(`${this.apiUrl}/${id}`, employee)
      .pipe(
        map(updatedEmployee => ({
          ...updatedEmployee,
          hireDate: new Date(updatedEmployee.hireDate),
          createdAt: new Date(updatedEmployee.createdAt),
          updatedAt: updatedEmployee.updatedAt ? new Date(updatedEmployee.updatedAt) : undefined
        })),
        catchError(this.handleError)
      );
  }

  deleteEmployee(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`)
      .pipe(
        catchError(this.handleError)
      );
  }

  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'An unknown error occurred!';
    
    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = `Error: ${error.error.message}`;
    } else {
      // Server-side error
      if (error.error && error.error.message) {
        errorMessage = error.error.message;
      } else if (error.message) {
        errorMessage = error.message;
      } else {
        errorMessage = `Error Code: ${error.status}\nMessage: ${error.statusText}`;
      }
    }
    
    console.error('EmployeeService Error:', error);
    return throwError(() => errorMessage);
  }
}