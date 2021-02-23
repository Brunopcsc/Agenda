using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Agenda.Models;

namespace Agenda.Controllers
{
    public class HomeController : Controller
    {
        private ContatoContext db = new ContatoContext();


        public ActionResult Login()
        {
            if (Session["usuarioLogadoID"] != null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(usuario u)
        {
            if (Session["usuarioLogadoID"] != null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                // esta action trata o post (login)
                if (ModelState.IsValid) //verifica se é válido
                {
                    using (CadastroEntities dc = new CadastroEntities())
                    {
                        var v = dc.usuarios.Where(a => a.NomeUsuario.Equals(u.NomeUsuario) && a.Senha.Equals(u.Senha)).FirstOrDefault();
                        if (v != null)
                        {
                            Session["usuarioLogadoID"] = v.Usuario_id.ToString();
                            Session["nomeUsuarioLogado"] = v.NomeUsuario.ToString();
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ViewBag.Message = "Usuário ou senha inválidos!";
                        }
                        
                    }
                }
                return View(u);
            }
        }

        //Logout
        public ActionResult Logout()
        {
            Session.Clear();//remove session
            return RedirectToAction("Login");
        }

        // GET: Home
        public ActionResult Index(string searchString)
        {
            
            if (Session["usuarioLogadoID"] == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                var contatos = from c in db.contatos
                               orderby c.Nome
                               select c;

                if (!String.IsNullOrEmpty(searchString))
                {
                    var pesquisa = searchString.ToUpper();
                    contatos = (IOrderedQueryable<Contato>)contatos.Where(s => s.Nome.ToUpper().Contains(pesquisa)).Union(contatos.Where(s => s.Cidade.ToUpper().Contains(pesquisa))).Union(contatos.Where(s => s.TelefoneCom.ToUpper().Contains(pesquisa))).Union(contatos.Where(s => s.Celular.ToUpper().Contains(pesquisa)));

                }
                return View(contatos.ToList());
            }
        }

        // GET: Home/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contato contato = db.contatos.Find(id);
            if (contato == null)
            {
                return HttpNotFound();
            }
            return View(contato);
        }

        // GET: Home/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Home/Create
        // Para proteger-se contra ataques de excesso de postagem, ative as propriedades específicas às quais deseja se associar. 
        // Para obter mais detalhes, confira https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Contato_id,Nome,Email,Endereco,Numero,Bairro,Cidade,Cep,TelefoneRes,TelefoneCom,Celular,OutrosNum,Telefone0800,Informacoes")] Contato contato)
        {
            if (ModelState.IsValid)
            {
                db.contatos.Add(contato);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(contato);
        }

        // GET: Home/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contato contato = db.contatos.Find(id);
            if (contato == null)
            {
                return HttpNotFound();
            }
            return View(contato);
        }

        // POST: Home/Edit/5
        // Para proteger-se contra ataques de excesso de postagem, ative as propriedades específicas às quais deseja se associar. 
        // Para obter mais detalhes, confira https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Contato_id,Nome,Email,Endereco,Numero,Bairro,Cidade,Cep,TelefoneRes,TelefoneCom,Celular,OutrosNum,Telefone0800,Informacoes")] Contato contato)
        {
            if (ModelState.IsValid)
            {
                db.Entry(contato).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(contato);
        }

        // GET: Home/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contato contato = db.contatos.Find(id);
            if (contato == null)
            {
                return HttpNotFound();
            }
            return PartialView(contato);
        }

        // POST: Home/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Contato contato = db.contatos.Find(id);
            db.contatos.Remove(contato);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
