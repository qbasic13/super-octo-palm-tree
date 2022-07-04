import { BookDetails } from "./books.model";

export interface OrderCreateRequest {
  orderItems: OrderItem[];
}

export interface OrderItem {
  isbn: string;
  quantity: number;
  details?: BookDetails;
}

export interface OrderOperationResponse {
  isSuccess: boolean;
  status: string;
  message?: string;
  order?: Order;
}

export interface Order {
  id: number,
  userEmail: string,
  userPhone: string,
  userFirstName: string,
  userLastName?: string,
  userMiddleName?: string,
  status: string,
  books?: BookDetails[],
  totalPrice: number,
  createdDate: Date,
  completionDate: Date
}
