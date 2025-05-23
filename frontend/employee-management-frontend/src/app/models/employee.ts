export interface Employee {
  id: number;
  firstName: string;
  lastName: string;
  fullName: string;
  email: string;
  phoneNumber?: string;
  department: string;
  position: string;
  salary: number;
  hireDate: Date;
  isActive: boolean;
  createdAt: Date;
  updatedAt?: Date;
}

export interface EmployeeListItem {
  id: number;
  fullName: string;
  email: string;
  department: string;
  position: string;
  isActive: boolean;
}

export interface CreateEmployeeRequest {
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  department: string;
  position: string;
  salary: number;
  hireDate: Date;
}

export interface UpdateEmployeeRequest {
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  department: string;
  position: string;
  salary: number;
  hireDate: Date;
  isActive: boolean;
}

export interface ApiResponse<T> {
  data?: T;
  message?: string;
  errors?: string[];
}