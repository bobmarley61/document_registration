create table if not exists users
(
    id serial ,
    login character varying (30),
    password character varying (100),
    fio character varying (30)
);
alter table users
    add primary key (id);

insert into users values (1, 'admin', 'admin', 'admin');
update users set password=MD5(password) where id = 1;

create table if not exists prikaz
(
    id_pr serial,
    date_pr character varying (100),
    index_pr character varying (100),
    text_pr character varying (100),
    after_text_pr character varying (100),
    date_insert character varying (100),
    fio character varying (100)
);

alter table prikaz
    add primary key (id_pr);

create table if not exists sluzebka
(
    id_sl serial,
    date_sl character varying (100),
    index_sl character varying (100),
    whom_sl character varying (100),
    text_sl character varying (100),
    after_text_sl character varying (100),
    date_insert character varying (100),
    fio character varying (100)
);

alter table sluzebka
    add primary key (id_sl);

create table if not exists inform
(
    id_inf serial,
    date_inf character varying (100),
    index_inf character varying (100),
    text_inf character varying (100),
    after_text_inf character varying (100),
    date_insert character varying (100),
    fio character varying (100)
);

alter table inform
    add primary key (id_inf);