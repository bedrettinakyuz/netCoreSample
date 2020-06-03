import { Component, OnInit, Input, Output , EventEmitter} from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { error } from '@angular/compiler/src/util';


@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  // homde dan gelen değer

  @Output() cancelRegister = new EventEmitter();
  model: any = {};

  constructor(private authServices:AuthService,private alertify: AlertifyService) { }

  ngOnInit() {
  }
  register(){

    this.authServices.register(this.model).subscribe(
      () => {
      this.alertify.success('Registiration is Succesfully');
    }, error => {
      this.alertify.error(error);
    }
  );
  }
  cancel(){
    this.cancelRegister.emit(false);

  }
}
