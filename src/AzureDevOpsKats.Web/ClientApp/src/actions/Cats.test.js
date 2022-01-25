import chai from 'chai';
import chaiHttp from 'chai-http';

const server = "http://localhost:5000";
chai.use(chaiHttp);

const id = '7867457364876923';
const id1 = '78674';
const page = 1;
const limit = 10;

//---------- Test the function deleteCatData() --------------

describe('Delete Cat', () => {
  it('Check delete cat is working or not', (done) => {

    // res.status must be 400 when id = '7867457364876923' (id format error)
    chai.request(server)
      .delete(`/api/v2/Cats/${id}`)
      .then(function (res) {
        chai.expect(res).to.have.status(400);
        done();
     })
     .catch(function (err) {
        done(err);
     });
    
    // res.status must be 400 when id = '78674' (no such item)
    chai.request(server)
      .delete(`/api/v2/Cats/${id1}`)
      .then(function (res) {
        chai.expect(res).to.have.status(404);
        done();
     })
     .catch(function (err) {
        done(err);
     });
  });
});

//------------- Test the function addCatData() ----------------------

const formData = {} // the parameters of post request
describe('Add Cat', () => {
  it('Check add cat is working or not', (done) => {
    // when the form is empty the res.status must be 400
    chai.request(server)
      .post(`/api/v2/Cats/`)
      .send(formData)
      .then(function (res) {
        chai.expect(res).to.have.status(400);
        done();
      })
      .catch(function (err) {
          done(err);
      });
    });
})

//------------- Test the function getCatData() ----------------------

describe('Get cat data', () => {
  it('Check cat data is working or not', (done) => {
    
    // res.status must be 400 when id = '7867457364876923' (id format error)
    chai.request(server)
      .get(`/api/v2/Cats/${id}`)
      .then(function (res) {
        chai.expect(res).to.have.status(400);
        done();
      })
      .catch(function (err) {
          done(err);
      });

    // res.status must be 400 when id = '78674' (no such item)
    chai.request(server)
      .get(`/api/v2/Cats/${id1}`)
      .then(function (res) {
        chai.expect(res).to.have.status(404);
        done();
      })
      .catch(function (err) {
          done(err);
      });
  });
});

//------------- Test the function getCats() ----------------------

describe('Get Cats', () => {

  // res.status must be 200 when limit = 10, page = 1
  it('Check Cats is working or not', (done) => {
    chai.request(server)
      .get(`/api/v2/Cats/${limit}/${page}`)
      .then(function (res) {
        chai.expect(res).to.have.status(200);
        done();
      })
      .catch(function (err) {
          done(err);
      });
  });
});


//------------- Test the function getCats() ----------------------

const updates = { // params of put request that includes update details
  name: 'aaa',
  description: 'aaa'    
}
describe('Update Cat', () => {
  it('Check update cat is working or not', (done) => {
    
    // res.status must be 400 when id = '7867457364876923' (id format error)
    chai.request(server)
      .put(`/api/v2/Cats/${id}`)
      .send(updates)
      .then(function (res) {
        chai.expect(res).to.have.status(400);
        done();
      })
      .catch(function (err) {
          done(err);
      });

    // res.status must be 400 when id = '78674' (no such item)
    chai.request(server)
      .put(`/api/v2/Cats/${id1}`)
      .send(updates)
      .then(function (res) {
        chai.expect(res).to.have.status(404);
        done();
      })
      .catch(function (err) {
          done(err);
      });
  });
});