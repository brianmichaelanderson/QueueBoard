import { AuthService } from './auth.service';

describe('AuthService dev helpers', () => {
  let svc: AuthService;

  beforeEach(() => {
    svc = new AuthService();
  });

  it('becomeAdmin() should cause isAdmin() to emit true', done => {
    svc.becomeAdmin();
    svc.isAdmin().subscribe(v => {
      expect(v).toBeTrue();
      done();
    });
  });

  it('becomeUser() should cause isAdmin() to emit false', done => {
    svc.becomeAdmin();
    svc.becomeUser();
    svc.isAdmin().subscribe(v => {
      expect(v).toBeFalse();
      done();
    });
  });
});
