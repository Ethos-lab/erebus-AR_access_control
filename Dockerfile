FROM ubuntu:20.04
ENV PATH="/root/miniconda3/bin:${PATH}"
ARG PATH="/root/miniconda3/bin:${PATH}"
ARG DEBIAN_FRONTEND=noninteractive
ENV TZ=Etc/UTC

RUN apt update && apt upgrade && apt-get clean
RUN apt-get install -y wget git nano

RUN cd /root && wget https://repo.anaconda.com/miniconda/Miniconda3-py38_23.3.1-0-Linux-x86_64.sh

RUN cd /root && mkdir /root/.conda && bash Miniconda3-py38_23.3.1-0-Linux-x86_64.sh -b

RUN bash -c "source /root/miniconda3/etc/profile.d/conda.sh && conda install -c conda-forge jupyterlab"
RUN bash -c "/root/miniconda3/bin/conda init bash"
RUN rm /root/Miniconda3-py38_23.3.1-0-Linux-x86_64.sh

RUN wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
RUN dpkg -i packages-microsoft-prod.deb

RUN printf 'Package: * \nPin: origin "packages.microsoft.com"\nPin-Priority: 1001' >> /etc/apt/preferences.d/99microsoft-dotnet.pref
RUN apt install -y apt-transport-https
RUN apt update
RUN apt install -y dotnet-sdk-6.0
RUN apt install -y dotnet-runtime-6.0
RUN rm packages-microsoft-prod.deb

RUN apt update
RUN apt-get install -y default-jdk

WORKDIR /root
RUN mkdir erebus
WORKDIR erebus

RUN git clone https://github.com/Ethos-lab/erebus-AR_access_control.git
RUN cd ./erebus-AR_access_control/policy_gen && pip install -r requirements.txt

RUN cd ./erebus-AR_access_control/policy_gen

WORKDIR erebus-AR_access_control
